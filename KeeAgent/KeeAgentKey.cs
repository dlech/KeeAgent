using System;
using dlech.PageantSharp;
using KeePassLib;

namespace KeeAgent
{
  /// <summary>
  /// Adds extra KeePass specific fields to PpkKey class
  /// </summary>
  public class KeeAgentKey : PpkKey
  {

    private PpkKey mPpkKey;

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
    /// <param name="aPpkKey"></param>
    public KeeAgentKey(PpkKey aPpkKey, string aDbPath, PwUuid aUuid, string aKeyFileName)
    {
      if (aPpkKey == null) {
        throw new ArgumentNullException("key");
      }
      if (aPpkKey == null) {
        throw new ArgumentNullException("dbPath");
      }
      if (aUuid == null) {
        throw new ArgumentNullException("uuid");
      }
      if (aKeyFileName == null) {
        throw new ArgumentNullException("keyFileName");
      }

      mPpkKey = aPpkKey; // keep reference to ppkKey so that it is not disposed by GC
      CipherKeyPair = aPpkKey.CipherKeyPair;
      Comment = aPpkKey.Comment;
      DbPath = aDbPath;
      Uuid = aUuid;
      KeyFileName = aKeyFileName;
    }
  }
}
