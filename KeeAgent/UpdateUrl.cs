/* this file is only included if KeePass version is >= 2.18 */
/* see PreBuild project for more information */

namespace KeeAgent
{
    partial class KeeAgentExt
    {
      // the #if UPDATE_URL directive is for simulating including or excluding  
      // this file during debugging/testing.
#if UPDATE_URL
        /// <summary>
        /// Returns url for automatic updating of plugin
        /// </summary> 
        public override string UpdateUrl
        {
            get { return "http://updates.lechnology.com/KeePassPlugins"; }
        }
#endif
    }
}
