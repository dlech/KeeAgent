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
    /// Show notification balloon when client uses key
    /// </summary>
    public bool ShowBalloon { get; set; }

    /// <summary>
    /// Cause all keys to have confirm constraint set even if it is not requested
    /// </summary>
    public bool AlwaysConfirm { get; set; }

    [Obsolete()]
    public NotificationOptions Notification { get; set; }

    public bool LoggingEnabled { get; set; }

    public string LogFileName { get; set; }

    public AgentMode AgentMode { get; set; }
        
  }
}
