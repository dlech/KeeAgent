using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace KeeAgent
{
  [Serializable]
  public class Options : ICloneable
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
    public bool AlwasyConfirm { get; set; }

    [Obsolete()]
    public NotificationOptions Notification { get; set; }

    public bool LoggingEnabled { get; set; }

    public string LogFileName { get; set; }

    public object Clone()
    {
      Options clone = new Options();
      clone.ShowBalloon = this.ShowBalloon;
      clone.AlwasyConfirm = this.AlwasyConfirm;
#pragma warning disable 0612
      clone.Notification = this.Notification;
#pragma warning restore 0612
      clone.LoggingEnabled = this.LoggingEnabled;
      clone.LogFileName = (string)LogFileName.Clone();
      return clone;
    }
  }
}
