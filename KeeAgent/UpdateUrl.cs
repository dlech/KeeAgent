
namespace KeeAgent
{
    partial class KeeAgentExt
    {
        /* only included if KeePass version is >= 2.18 */
        /* see PreBuild for workaround */
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
