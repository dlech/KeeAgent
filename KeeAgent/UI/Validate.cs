// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2022 David Lechner <david@lechnology.com>

using System;
using System.IO;
using KeePassLib.Security;
using SshAgentLib.Keys;

namespace KeeAgent.UI
{
  /// <summary>
  /// Miscellaneous validators.
  /// </summary>
  public static class Validate
  {
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

        SshPublicKey publicKey = null;

        var pubAttachment = getAttachment(location.AttachmentName + ".pub");

        if (pubAttachment != null) {
          publicKey = SshPublicKey.Read(new MemoryStream(pubAttachment.ReadData()));
        }

        try {
          SshPrivateKey.Read(new MemoryStream(attachment.ReadData()), publicKey);
        }
        catch (SshPrivateKey.PublicKeyRequiredException) {
          return string.Format(
            "This file uses a legacy format that requires a '{0}.pub' file in the same location.",
            location.AttachmentName);
        }
        catch {
          return string.Format(
            "attachment '{0}' is not a supported private key file.", location.AttachmentName);
        }
      }

      if (location.SelectedType.Value == EntrySettings.LocationType.File) {
        var file = location.FileName.ExpandEnvironmentVariables();

        if (!File.Exists(file)) {
          return string.Format("The private key file '{0}' does not exist.", file);
        }

        SshPublicKey publicKey = null;

        if (File.Exists(file + ".pub")) {
          publicKey = SshPublicKey.Read(File.OpenRead(file + ".pub"));
        }

        try {
          SshPrivateKey.Read(File.OpenRead(file), publicKey);
        }
        catch (SshPrivateKey.PublicKeyRequiredException) {
          return string.Format(
            "This file uses a legacy format that requires a '{0}.pub' file in the same location.",
            file);
        }
        catch {
          return string.Format("'{0}' is not a supported private key file.", file);
        }
      }

      return null;
    }
  }
}
