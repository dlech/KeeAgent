using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeeAgent
{
  public class EntrySettings
  {
    public bool HasSshKey { get; set; }
    public bool LoadAtStartup { get; set; }

    public EntrySettings()
    {
      HasSshKey = false;
      LoadAtStartup = false;
    }
  }
}
