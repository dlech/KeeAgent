using System;
using dlech.SshAgentLib;
using KeePassLib;
using System.Collections.ObjectModel;
using Org.BouncyCastle.Crypto;

namespace KeeAgent
{
  /// <summary>
  /// Adds extra KeePass specific fields to PpkKey class
  /// </summary>
  public class KeeAgentKey : ISshKey
  {

    private ISshKey mSshKey;

    /// <summary>
    /// The the full path name of the db associated with this key
    /// </summary>
    public string DbPath { get; private set; }

    /// <summary>
    /// The Uuid of the PwEntry associated with this key
    /// </summary>
    public PwUuid Uuid { get; private set; }

    /// <summary>
    /// File name of the key (key field of ProtectedBinary dictionary)
    /// </summary>
    public string KeyFileName { get; private set; }

    public bool IsPublicOnly { get { return false; } }

    /// <summary>
    /// create new instance of KeeAgentKey
    /// </summary>
    /// <param name="aSshKey"></param>
    public KeeAgentKey(ISshKey aSshKey, string aDbPath, PwUuid aUuid, string aKeyFileName)
    {
      if (aSshKey == null) {
        throw new ArgumentNullException("key");
      }
      if (aSshKey == null) {
        throw new ArgumentNullException("dbPath");
      }
      if (aUuid == null) {
        throw new ArgumentNullException("uuid");
      }
      if (aKeyFileName == null) {
        throw new ArgumentNullException("keyFileName");
      }

      mSshKey = aSshKey; // keep reference to ppkKey so that it is not disposed by GC
      DbPath = aDbPath;
      Uuid = aUuid;
      KeyFileName = aKeyFileName;
    }

    public PublicKeyAlgorithm Algorithm
    {
      get { return mSshKey.Algorithm; }
    }

    public string Comment
    {
      get
      {
        return mSshKey.Comment;
      }
      set
      {
        mSshKey.Comment = value;
      }
    }

    public ReadOnlyCollection<Agent.KeyConstraint> Constraints
    {
      get { return mSshKey.Constraints; }
    }

    public AsymmetricKeyParameter GetPrivateKeyParameters()
    {
      return mSshKey.GetPrivateKeyParameters();
    }

    public AsymmetricKeyParameter GetPublicKeyParameters()
    {
      return mSshKey.GetPublicKeyParameters();
    }

    public byte[] MD5Fingerprint
    {
      get { return mSshKey.MD5Fingerprint; }
    }

    public int Size
    {
      get { return mSshKey.Size; }
    }

    public SshVersion Version
    {
      get { return mSshKey.Version; }
    }

    public void AddConstraint(Agent.KeyConstraint aConstraint)
    {
      mSshKey.AddConstraint(aConstraint);
    }

    public void Dispose()
    {
      mSshKey.Dispose();
    }
  }
}
