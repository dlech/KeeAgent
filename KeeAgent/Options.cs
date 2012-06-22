using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace KeeAgent
{
    public class Options : ICloneable
    {
        public Options()
        {
            /* set default values */
            Notification = NotificationOptions.Balloon;
        }

        public NotificationOptions Notification { get; set; }

        public bool LoggingEnabled { get; set; }

        public string LogFileName { get; set; }

        public object Clone()
        {
            Options clone = new Options();
            clone.Notification = this.Notification;
            clone.LoggingEnabled = this.LoggingEnabled;
            clone.LogFileName = (string)LogFileName.Clone();
            return clone;
        }
    }
}
