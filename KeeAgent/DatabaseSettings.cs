using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace KeeAgent
{
  public class DatabaseSettings
  {
    public bool UnlockOnActivity { get; set; }

    public DatabaseSettings()
    {
      UnlockOnActivity = true;
    }
  }
}
