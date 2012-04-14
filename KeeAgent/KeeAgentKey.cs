using System;
using System.Collections.Generic;
using System.Linq;
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
		/// The Uuid of the PwEntry associated with this key
		/// </summary>
		public PwUuid Uuid { get; private set; }

		/// <summary>
		/// File name of the key (key field of ProtectedBinary dictionary)
		/// </summary>
		public string Filename { get; private set; }
			

		/// <summary>
		/// create new instance of KeeAgentKey
		/// </summary>
		/// <param name="ppkKey"></param>
		public KeeAgentKey(PpkKey ppkKey, PwUuid uuid, string fileName)
		{
			if (ppkKey == null) {
				throw new ArgumentNullException("key");
			}
			if (uuid == null) {
				throw new ArgumentNullException("uuid");
			}
			if (fileName == null) {
				throw new ArgumentNullException("fileName");
			}

			this.ppkKey = ppkKey; // keep reference to ppkKey so that it is not disposed by GC
			this.Algorithm = ppkKey.Algorithm;
			this.Comment = ppkKey.Comment;
			this.Uuid = uuid;
			this.Filename = fileName;
		}
	}
}
