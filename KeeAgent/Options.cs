// SPDX-License-Identifier: GPL-2.0-only
// Copyright (c) 2012-2017,2022 David Lechner <david@lechnology.com>

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
    /// When true and using PageantAgent, a socket file will be created at
    /// CygwinSocketPath that can be used with cygwin
    /// </summary>
    public bool UseCygwinSocket { get; set; }

    /// <summary>
    /// The path for the file created when <see cref="UseCygwinSocket"/> is enabled.
    /// </summary>
    public string CygwinSocketPath { get; set; }

    /// <summary>
    /// When true and using PageantAgent, a socket file will be created at
    /// MsysSocketPath that can be used with msys
    /// </summary>
    public bool UseMsysSocket { get; set; }

    /// <summary>
    /// The path for the file created when <see cref="UseMsysSocket"/> is enabled.
    /// </summary>
    public string MsysSocketPath { get; set; }

    /// <summary>
    /// When true and using PageantAgent, a socket file will be created at
    /// UnixSocketPath that can be used with WSL
    /// </summary>
    public bool UseWslSocket { get; set; }

    /// <summary>
    /// The path for the file created when <see cref="UseWslSocket"/> is enabled.
    /// </summary>
    public string WslSocketPath { get; set; }

    /// <summary>
    /// When <c>true</c> and using PageantAgent, a named pipe will be created
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

    /// <summary>
    /// When <c>true</c>, we will not display a progress bar during SSH key decryption.
    /// </summary>
    public bool DisableKeyDecryptionProgressBar { get; set; }
  }
}
