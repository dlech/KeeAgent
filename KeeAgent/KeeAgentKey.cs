using System;
using System.Collections.Generic;
using System.Text;
using dlech.PageantSharp;
using KeePassLib;
using System.Security.Cryptography;

namespace KeeAgent
{
	/// <summary>
	/// Adds extra KeePass specific fields to PpkKey class
	/// </summary>
	public class KeeAgentKey : PpkKey
	{

		private PpkKey ppkKey;

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
		/// <param name="ppkKey"></param>
		public KeeAgentKey(PpkKey ppkKey, string dbPath, PwUuid uuid, string keyFileName)
		{
			if (ppkKey == null) {
				throw new ArgumentNullException("key");
			}
            if (ppkKey == null) {
                throw new ArgumentNullException("dbPath");
            }
			if (uuid == null) {
				throw new ArgumentNullException("uuid");
			}
			if (keyFileName == null) {
                throw new ArgumentNullException("keyFileName");
			}

			this.ppkKey = ppkKey; // keep reference to ppkKey so that it is not disposed by GC
            this.KeyParameters = ppkKey.KeyParameters;
			this.Comment = ppkKey.Comment;
            this.DbPath = dbPath;
			this.Uuid = uuid;
			this.KeyFileName = keyFileName;
		}
	}
}
