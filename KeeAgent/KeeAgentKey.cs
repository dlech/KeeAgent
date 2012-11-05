using System;
using dlech.PageantSharp;
using KeePassLib;

namespace KeeAgent
{
  /// <summary>
  /// Adds extra KeePass specific fields to PpkKey class
  /// </summary>
  public class KeeAgentKey : SshKey
  {

    private SshKey mSshKey;

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


    /// <summary>
    /// create new instance of KeeAgentKey
    /// </summary>
    /// <param name="aSshKey"></param>
    public KeeAgentKey(SshKey aSshKey, string aDbPath, PwUuid aUuid, string aKeyFileName)
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
      Version = aSshKey.Version;
      CipherKeyPair = aSshKey.CipherKeyPair;
      Comment = aSshKey.Comment;
      DbPath = aDbPath;
      Uuid = aUuid;
      KeyFileName = aKeyFileName;
    }
  }
}
