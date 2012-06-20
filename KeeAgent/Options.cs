using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace KeeAgent
{
    public class Options
    {
        public Options()
        {
            /* set default values */
            Notification = NotificationOptions.Balloon;
        }

        public NotificationOptions Notification { get; set;}
    }
}
