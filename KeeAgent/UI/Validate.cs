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
        using (var reader = new StreamReader(stream)) {
          KeyFormatter.GetFormatter(reader.ReadLine());
          return true;
        }
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
    /// <param name="isPrivate">
    /// If <c>true</c>, this is the private key data, otherwise it is the public key data.
    /// </param>
    /// <returns>
    /// <c>null</c> if the data is valid, otherwise an error message suitable for display to the user.
    /// </returns>
    public static string Location(
      EntrySettings.LocationData location,
      Func<string, ProtectedBinary> getAttachment,
      bool isPrivate)
    {
      // we don't actually expect this to happen
      if (location == null) {
        return null;
      }

      var type = isPrivate ? "private" : "public";

      if (!location.SelectedType.HasValue) {
        return string.Format(
          "No {0} key file type is selected. Select an option and try again.",
          type);
      }

      if (location.SelectedType.Value == EntrySettings.LocationType.Attachment) {
        var attachment = getAttachment(location.AttachmentName);

        if (attachment == null) {
          return string.Format(
            "Attachment '{0}' is missing.", location.AttachmentName);
        }

        var stream = new MemoryStream(attachment.ReadData());

        if (isPrivate) {
          if (!SshPrivateKeyFile(stream)) {
            return string.Format(
              "attachment '{0}' is not a supported private key file.", location.AttachmentName);
          }
        }
        else {
          if (!SshPublicKeyFile(stream)) {
            return string.Format(
              "attachment '{0}' is not a supported public key file.", location.AttachmentName);
          }
        }
      }

      if (location.SelectedType.Value == EntrySettings.LocationType.File) {
        var file = location.FileName.ExpandEnvironmentVariables();

        if (File.Exists(file)) {
          if (isPrivate) {
            if (!SshPrivateKeyFile(File.OpenRead(file))) {
              return string.Format(
                "'{0}' is not a supported private key file.", file);
            }
          }
          else {
            if (!SshPublicKeyFile(File.OpenRead(file))) {
              return string.Format(
                "'{0}' is not a supported public key file.", file);
            }
          }
        }
        else {
          return string.Format(
            "The {0} key file '{1}' does not exist.",
            type, file);
        }
      }

      return null;
    }
  }
}
