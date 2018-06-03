//
//  Options.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2012-2013  David Lechner
//
//  This program is free software; you can redistribute it and/or
//  modify it under the terms of the GNU General Public License
//  as published by the Free Software Foundation; either version 2
//  of the License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, see <http://www.gnu.org/licenses>

using System;
using dlech.SshAgentLib;

namespace KeeAgent
{
  [Serializable]
  public class Options
  {
    public Options()
    {
      /* set default values */
    }

    /// <summary>
    /// Show notification balloon when client uses keys (Agent mode only)
    /// </summary>
    public bool ShowBalloon { get; set; }

    /// <summary>
    /// Cause all keys to have confirm constraint set even if it is not requested (Agent mode only)
    /// </summary>
    public bool AlwaysConfirm { get; set; }

    /// <summary>
    /// Replaced by ShowBalloon and AlwaysConfirm
    /// </summary>
    [Obsolete()]
    public NotificationOptions Notification { get; set; }

    /// <summary>
    /// Turns on debug logging - currently not working
    /// </summary>
    public bool LoggingEnabled { get; set; }

    /// <summary>
    /// The file name for the debug log file
    /// </summary>
    public string LogFileName { get; set; }

    /// <summary>
    /// Specifies which mode to run the agent in.
    /// </summary>
    public AgentMode AgentMode { get; set; }

    /// <summary>
    /// When true, all databases will be unlocked when an SSH keys is requested (Agent mode only).
    /// </summary>
    public bool UnlockOnActivity { get; set; }

    /// <summary>
    /// When true and using PagentAgent, a socket file will be created at
    /// CygwinSocketPath that can be used with cygwin
    /// </summary>
    public bool UseCygwinSocket { get; set; }

    /// <summary>
    /// The path for the file created when using UseCygwinSocket
    /// </summary>
    public string CygwinSocketPath { get; set; }

    /// <summary>
    /// When true and using PagentAgent, a socket file will be created at
    /// MsysSocketPath that can be used with msys
    /// </summary>
    public bool UseMsysSocket { get; set; }

    /// <summary>
    /// The path for the file created when using UseCygwinSocketUseMsysSocket
    /// </summary>
    public string MsysSocketPath { get; set; }

    /// <summary>
    /// When <c>true</c> and using PagentAgent, a named pipe will be created
    /// at //./pipe/openssh-ssh-agent that can be used with Windows OpenSSH
    /// </summary>
    public bool UseWindowsOpenSshPipe { get; set; }

    /// <summary>
    /// The path for creating a unix socket in Agent mode on unix-like platforms
    /// </summary>
    /// <value>The unix socket path.</value>
    public string UnixSocketPath { get; set; }

    /// <summary>
    /// When <c>true</c>, the user will be prompted to select a keys from the
    /// loaded keys in response to a client program requesting a list of
    /// identities.
    /// </summary>
    public static bool UserPicksKeyOnRequestIdentities { get; set; }

    /// <summary>
    /// When <c>true</c>, we will not display a warning about missing keyfiles
    /// in the filesystem.
    /// </summary>
    public static bool IgnoreMissingExternalKeyFiles { get; set; }
  }
}
