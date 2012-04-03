using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeeAgent
{
	public static class KeeAgentUtil
	{
		private const string defaultDelimeter = ":";

		/// <summary>
		/// Formats MAC with the pattern 2 chars, delimeter, 2 chars, delimeter ... 1 or 2 chars
		/// Using the default delimeter of ':'
		/// </summary>
		/// <param name="MAC">the string to be formatted</param>
		/// <returns>the formatted string</returns>
		/// <exception cref="System.ArgumentNullException">thrown if MAC is null</exception>
		public static string FormatMAC(string MAC)
		{
			return FormatMAC(MAC, defaultDelimeter);
		}

		/// <summary>
		/// Formats MAC with the pattern 2 chars, delimeter, 2 chars, delimeter ... 1 or 2 chars
		/// </summary>
		/// <param name="MAC">the string to be formatted</param>
		/// <param name="delimeter">the delimeter to insert</param>
		/// <returns>the formatted string</returns>
		/// <exception cref="System.ArgumentNullException">thrown if MAC or delimeter is null</exception>
		public static string FormatMAC(string MAC, string delimeter)
		{
			if (MAC == null) {
				throw new ArgumentNullException("MAC");
			}
			if (delimeter == null) {
				throw new ArgumentNullException("delimeter");
			}
			
			int macLength = MAC.Length;
			string[] charGroupArray = new string[macLength / 2 + macLength % 2];
			for (int i = 0; i < macLength; i += 2) {
				charGroupArray[i / 2] = MAC.Substring(i, Math.Min(macLength - i, 2));
			}
			return string.Join(delimeter, charGroupArray);
		}
	}
}
