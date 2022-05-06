// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2022 David Lechner <david@lechnology.com>

using System;
using System.IO;
using KeePassLib.Security;
using SshAgentLib.Keys;
using dlech.SshAgentLib;

namespace KeeAgent.UI
{
  /// <summary>
  /// Miscellaneous validators.
  /// </summary>
  public static class Validate
  {
    /// <summary>
    /// Validates that <paramref name="stream"/> is a supported SSH public key file.
    /// </summary>
    /// <param name="stream">The stream containg the file data.</param>
    /// <returns><c>true</c> if the file is valid, otherwise <c>false</c>.</returns>
    public static bool SshPublicKeyFile(Stream stream)
    {
      try {
        SshPublicKey.Read(stream);
        return true;
      }
      catch {
        return false;
      }
    }

    public static bool SshPrivateKeyFile(Stream stream)
    {
      try {
        SshPrivateKey.Read(stream);
        return true;
      }
      catch {
        return false;
      }
    }

    /// <summary>
    /// Validates the location data.
    /// </summary>
    /// <param name="location">
    /// The location data to check.
    /// </param>
    /// <param name="getAttachment">
    /// Callback function to get the attachment by name.
    /// </param>
    /// <returns>
    /// <c>null</c> if the data is valid, otherwise an error message suitable for display to the user.
    /// </returns>
    public static string Location(
      EntrySettings.LocationData location,
      Func<string, ProtectedBinary> getAttachment)
    {
      // we don't actually expect this to happen
      if (location == null) {
        return null;
      }

      if (!location.SelectedType.HasValue) {
        return "No private key file type is selected. Select an option and try again.";
      }

      if (location.SelectedType.Value == EntrySettings.LocationType.Attachment) {
        var attachment = getAttachment(location.AttachmentName);

        if (attachment == null) {
          return string.Format(
            "Attachment '{0}' is missing.", location.AttachmentName);
        }

        var stream = new MemoryStream(attachment.ReadData());

        if (!SshPrivateKeyFile(stream)) {
          return string.Format(
            "attachment '{0}' is not a supported private key file.", location.AttachmentName);
        }
      }

      if (location.SelectedType.Value == EntrySettings.LocationType.File) {
        var file = location.FileName.ExpandEnvironmentVariables();

        if (File.Exists(file)) {
          if (!SshPrivateKeyFile(File.OpenRead(file))) {
            return string.Format(
              "'{0}' is not a supported private key file.", file);
          }
        }
        else {
          return string.Format(
            "The private key file '{0}' does not exist.",
            file);
        }
      }

      return null;
    }
  }
}
