using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
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
    /// Show notification balloon when client uses key (Agent mode only)
    /// </summary>
    public bool ShowBalloon { get; set; }

    /// <summary>
    /// Cause all keys to have confirm constraint set even if it is not requested (Agent mode only)
    /// </summary>
    public bool AlwaysConfirm { get; set; }

    /// <summary>
    /// Replaced by ShowBallon and AlwaysConfirm
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

  }
}
