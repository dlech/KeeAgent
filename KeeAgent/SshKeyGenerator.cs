// SPDX-License-Identifier: GPL-2.0-only
using System;
using System.IO;
using System.Text;
using KeePassLib.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SshAgentLib.Keys;

namespace KeeAgent
{
  internal static class SshKeyGenerator
  {
    internal static void Generate(string comment,
      out byte[] privateKeyBytes, out byte[] publicKeyBytes)
    {
      if (comment == null) comment = string.Empty;
      if (comment.IndexOfAny(new char[] { '\r', '\n', '\0' }) >= 0) {
        throw new ArgumentException("comment must not contain newlines or NUL", "comment");
      }
      // No-arg constructor seeds from RNGCryptoServiceProvider.
      // SetSeed() ADDS entropy — it does not replace existing seeding.
      var secureRandom = new SecureRandom();
      secureRandom.SetSeed(CryptoRandom.Instance.GetRandomBytes(64));

      var privateParams = new Ed25519PrivateKeyParameters(secureRandom);
      var seed = privateParams.GetEncoded();                     // 32 bytes
      var pub  = privateParams.GeneratePublicKey().GetEncoded(); // 32 bytes

      try {
        privateKeyBytes = BuildPrivateKey(seed, pub, comment);
        publicKeyBytes  = BuildPublicKey(pub, comment);
      }
      finally {
        Array.Clear(seed, 0, seed.Length);
      }
    }

    static byte[] BuildPrivateKey(byte[] seed, byte[] pub, string comment)
    {
      var commentBytes = Encoding.UTF8.GetBytes(comment);
      var checkInt = CryptoRandom.Instance.GetRandomBytes(4);

      using (var blob = new MemoryStream()) {
        // magic
        blob.Write(Encoding.ASCII.GetBytes("openssh-key-v1"), 0, 14);
        blob.WriteByte(0);

        WriteString(blob, "none"); // cipher
        WriteString(blob, "none"); // kdf
        WriteString(blob, new byte[0]); // kdf options (empty)

        WriteUInt32(blob, 1); // number of keys

        // public key blob
        using (var pubBlob = new MemoryStream()) {
          WriteString(pubBlob, "ssh-ed25519");
          WriteString(pubBlob, pub);
          WriteString(blob, pubBlob.ToArray());
        }

        // private key blob
        using (var privBlob = new MemoryStream()) {
          privBlob.Write(checkInt, 0, 4); // check_int repeated twice
          privBlob.Write(checkInt, 0, 4);
          WriteString(privBlob, "ssh-ed25519");
          WriteString(privBlob, pub);
          var privMaterial = new byte[64]; // seed (32) || pubkey (32)
          try {
            Buffer.BlockCopy(seed, 0, privMaterial,  0, 32);
            Buffer.BlockCopy(pub,  0, privMaterial, 32, 32);
            WriteString(privBlob, privMaterial);
          }
          finally {
            Array.Clear(privMaterial, 0, privMaterial.Length);
          }
          WriteString(privBlob, commentBytes);
          for (int pad = 1; privBlob.Length % 8 != 0; pad++) {
            privBlob.WriteByte((byte)pad);
          }
          WriteString(blob, privBlob.ToArray());
        }

        var b64 = Convert.ToBase64String(blob.ToArray());
        var sb = new StringBuilder();
        sb.Append("-----BEGIN OPENSSH PRIVATE KEY-----\n");
        for (int i = 0; i < b64.Length; i += 70) {
          sb.Append(b64.Substring(i, Math.Min(70, b64.Length - i))).Append('\n');
        }
        sb.Append("-----END OPENSSH PRIVATE KEY-----\n");
        return Encoding.ASCII.GetBytes(sb.ToString());
      }
    }

    static byte[] BuildPublicKey(byte[] pub, string comment)
    {
      using (var keyBlob = new MemoryStream()) {
        WriteString(keyBlob, "ssh-ed25519");
        WriteString(keyBlob, pub);
        var b64 = Convert.ToBase64String(keyBlob.ToArray());
        return Encoding.UTF8.GetBytes("ssh-ed25519 " + b64 + " " + comment);
      }
    }

    static void WriteString(Stream s, string value)
    {
      WriteString(s, Encoding.UTF8.GetBytes(value));
    }

    static void WriteString(Stream s, byte[] value)
    {
      WriteUInt32(s, (uint)value.Length);
      s.Write(value, 0, value.Length);
    }

    static void WriteUInt32(Stream s, uint value)
    {
      s.WriteByte((byte)(value >> 24));
      s.WriteByte((byte)(value >> 16));
      s.WriteByte((byte)(value >> 8));
      s.WriteByte((byte)value);
    }

    internal static void ChangeComment(
      byte[] attachmentBytes,
      string newComment,
      out byte[] privateKeyBytes,
      out byte[] publicKeyBytes)
    {
      if (newComment == null) newComment = string.Empty;

      SshPrivateKey key;
      using (var ms = new MemoryStream(attachmentBytes)) {
        key = SshPrivateKey.Read(ms);
      }

      if (key.IsEncrypted) {
        throw new InvalidOperationException(
          "Key is encrypted. Decrypt it first to change the comment.");
      }

      string oldComment;
      var privateParams = key.Decrypt(() => new byte[0], null, out oldComment);

      var authParts = key.PublicKey.AuthorizedKeysString.Split(' ');
      var algo = authParts[0];
      var b64Pub = authParts[1];
      var pubKeyBlob = Convert.FromBase64String(b64Pub);

      privateKeyBytes = BuildDecryptedPrivateKey(privateParams, pubKeyBlob, newComment);

      publicKeyBytes = Encoding.UTF8.GetBytes(
        string.IsNullOrEmpty(newComment)
          ? algo + " " + b64Pub
          : algo + " " + b64Pub + " " + newComment);
    }

    internal static void Decrypt(
      byte[] attachmentBytes,
      string passphrase,
      out byte[] privateKeyBytes,
      out byte[] publicKeyBytes)
    {
      var pass = Encoding.UTF8.GetBytes(passphrase ?? string.Empty);
      try {
        SshPrivateKey key;
        using (var ms = new MemoryStream(attachmentBytes)) {
          key = SshPrivateKey.Read(ms);
        }

        if (!key.IsEncrypted) {
          throw new InvalidOperationException("Key is not encrypted.");
        }

        string comment;
        var capturedPass = pass;
        var privateParams = key.Decrypt(
          () => capturedPass, null, out comment);

        // AuthorizedKeysString = "{algo} {base64(KeyBlob)} [{comment}]"
        // KeyBlob is internal, so decode via AuthorizedKeysString.
        var authParts = key.PublicKey.AuthorizedKeysString.Split(' ');
        var algo = authParts[0];
        var b64Pub = authParts[1];
        var pubKeyBlob = Convert.FromBase64String(b64Pub);

        privateKeyBytes = BuildDecryptedPrivateKey(privateParams, pubKeyBlob, comment);

        publicKeyBytes = Encoding.UTF8.GetBytes(
          string.IsNullOrEmpty(comment)
            ? algo + " " + b64Pub
            : algo + " " + b64Pub + " " + comment);
      }
      finally {
        Array.Clear(pass, 0, pass.Length);
      }
    }

    static byte[] BuildDecryptedPrivateKey(
      AsymmetricKeyParameter privateParams, byte[] pubKeyBlob, string comment)
    {
      var commentBytes = Encoding.UTF8.GetBytes(comment ?? string.Empty);
      var checkInt = CryptoRandom.Instance.GetRandomBytes(4);

      using (var blob = new MemoryStream()) {
        blob.Write(Encoding.ASCII.GetBytes("openssh-key-v1"), 0, 14);
        blob.WriteByte(0);

        WriteString(blob, "none");
        WriteString(blob, "none");
        WriteString(blob, new byte[0]);
        WriteUInt32(blob, 1);
        WriteString(blob, pubKeyBlob);

        using (var privBlob = new MemoryStream()) {
          privBlob.Write(checkInt, 0, 4);
          privBlob.Write(checkInt, 0, 4);
          privBlob.Write(pubKeyBlob, 0, pubKeyBlob.Length);
          WriteDecryptedKeyMaterial(privBlob, privateParams);
          WriteString(privBlob, commentBytes);
          for (int pad = 1; privBlob.Length % 8 != 0; pad++) {
            privBlob.WriteByte((byte)pad);
          }
          WriteString(blob, privBlob.ToArray());
        }

        var b64 = Convert.ToBase64String(blob.ToArray());
        var sb = new StringBuilder();
        sb.Append("-----BEGIN OPENSSH PRIVATE KEY-----\n");
        for (int i = 0; i < b64.Length; i += 70) {
          sb.Append(b64.Substring(i, Math.Min(70, b64.Length - i))).Append('\n');
        }
        sb.Append("-----END OPENSSH PRIVATE KEY-----\n");
        return Encoding.ASCII.GetBytes(sb.ToString());
      }
    }

    static void WriteDecryptedKeyMaterial(Stream s, AsymmetricKeyParameter key)
    {
      var ed25519 = key as Ed25519PrivateKeyParameters;
      if (ed25519 != null) {
        var seed = ed25519.GetEncoded();
        var pub = ed25519.GeneratePublicKey().GetEncoded();
        var combined = new byte[64];
        try {
          Buffer.BlockCopy(seed, 0, combined, 0, 32);
          Buffer.BlockCopy(pub, 0, combined, 32, 32);
          WriteString(s, combined);
        }
        finally {
          Array.Clear(seed, 0, seed.Length);
          Array.Clear(combined, 0, combined.Length);
        }
        return;
      }

      var rsa = key as RsaPrivateCrtKeyParameters;
      if (rsa != null) {
        // order: d, iqmp, p, q (matches OpenSSH wire format)
        WriteString(s, rsa.Exponent.ToByteArrayUnsigned());
        WriteString(s, rsa.QInv.ToByteArrayUnsigned());
        WriteString(s, rsa.P.ToByteArrayUnsigned());
        WriteString(s, rsa.Q.ToByteArrayUnsigned());
        return;
      }

      var ecdsa = key as ECPrivateKeyParameters;
      if (ecdsa != null) {
        WriteString(s, ecdsa.D.ToByteArrayUnsigned());
        return;
      }

      var dsa = key as DsaPrivateKeyParameters;
      if (dsa != null) {
        WriteString(s, dsa.X.ToByteArrayUnsigned());
        return;
      }

      var ed448 = key as Ed448PrivateKeyParameters;
      if (ed448 != null) {
        var seed = ed448.GetEncoded();
        var pub = ed448.GeneratePublicKey().GetEncoded();
        var combined = new byte[seed.Length + pub.Length];
        try {
          Buffer.BlockCopy(seed, 0, combined, 0, seed.Length);
          Buffer.BlockCopy(pub, 0, combined, seed.Length, pub.Length);
          WriteString(s, combined);
        }
        finally {
          Array.Clear(seed, 0, seed.Length);
          Array.Clear(combined, 0, combined.Length);
        }
        return;
      }

      throw new NotSupportedException("Unsupported key type: " + key.GetType().Name);
    }
  }
}
