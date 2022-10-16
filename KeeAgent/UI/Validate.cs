﻿// SPDX-License-Identifier: GPL-2.0-only
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

        SshPrivateKey privateKey;

        try {
          privateKey = SshPrivateKey.Read(new MemoryStream(attachment.ReadData()));
        }
        catch {
          return string.Format(
            "attachment '{0}' is not a supported private key file.", location.AttachmentName);
        }

        SshPublicKey publicKey = privateKey.PublicKey;

        var certAttachment = getAttachment(location.AttachmentName + "-cert.pub");

        if (certAttachment != null) {
          publicKey = SshPublicKey.Read(new MemoryStream(certAttachment.ReadData()));
        }

        var pubAttachment = getAttachment(location.AttachmentName + ".pub");

        if (pubAttachment != null) {
          publicKey = SshPublicKey.Read(new MemoryStream(pubAttachment.ReadData()));
        }

        if (publicKey == null) {
          return string.Format(
            "This file uses a legacy format that requires a '{0}.pub' file in the same location.",
            location.AttachmentName);
        }
      }

      if (location.SelectedType.Value == EntrySettings.LocationType.File) {
        var file = location.FileName.ExpandEnvironmentVariables();

        if (!File.Exists(file)) {
          return string.Format("The private key file '{0}' does not exist.", file);
        }

        SshPrivateKey privateKey;

        try {
          privateKey = SshPrivateKey.Read(File.OpenRead(file));
        }
        catch {
          return string.Format("'{0}' is not a supported private key file.", file);
        }

        SshPublicKey publicKey = privateKey.PublicKey;

        if (File.Exists(file + "-cert.pub")) {
          publicKey = SshPublicKey.Read(File.OpenRead(file + "-cert.pub"));
        }
        else if (File.Exists(file + ".pub")) {
          publicKey = SshPublicKey.Read(File.OpenRead(file + ".pub"));
        }

        if (publicKey == null) {
          return string.Format(
            "This file uses a legacy format that requires a '{0}.pub' file in the same location.",
            file);
        }
      }

      return null;
    }
  }
}
