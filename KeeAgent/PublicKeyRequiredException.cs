// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2022 David Lechner <david@lechnology.com>

using System;

namespace KeeAgent
{
  [Serializable]
  internal sealed class PublicKeyRequiredException : Exception
  {
    public PublicKeyRequiredException()
    {
    }
  }
}
