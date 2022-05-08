// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2022 David Lechner <david@lechnology.com>

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KeePassLib.Collections;
using KeePassLib.Security;

namespace KeeAgent.UI
{
  /// <summary>
  /// Binding list wrapper for PwEntry attachments.
  /// </summary>
  public class AttachmentBindingList : BindingList<KeyValuePair<string, ProtectedBinary>>
  {
    /// <summary>
    /// Creates a new list.
    /// </summary>
    /// <param name="binaries">The binary dictionary.</param>
    public AttachmentBindingList(ProtectedBinaryDictionary binaries) : base(binaries.ToList())
    {
      AllowEdit = false;
      AllowNew = false;
      AllowRemove = false;
    }
  }
}
