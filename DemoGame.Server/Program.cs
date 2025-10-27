using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DemoGame.Server.Queries;
using DemoGame.Server.UI;
using NetGore.Db;
using log4net;
using NetGore.IO;

namespace DemoGame.Server
{
    class Program
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Calls <see cref="ContentPaths.TryCopyContent"/> in debug builds.
        /// </summary>
        [Conditional("DEBUG")]
        static void CopyContent()
        {
            if (ContentPaths.TryCopyContent(userArgs: CommonConfig.TryCopyContentArgs))
            {
                if (log.IsInfoEnabled)
                    log.Info("TryCopyContent succeeded");
            }
            else
            {
                if (log.IsInfoEnabled)
                    log.Info("TryCopyContent failed");
            }
        }

        /// <summary>
        /// Server program entry point.
        /// </summary>
        static void Main(string[] args)
        {
            // Check to apply patches
            if (args != null && args.Any(x => x != null && x.Trim() == "--patchdb"))
            {
                ServerDbPatcher.ApplyPatches();
                return;
            }

            Console.Title = "NetGore Server";

            // Copy content
            CopyContent();

            // Check to run in compact mode
            bool useCompactMode = (args != null && args.Any(x => StringComparer.OrdinalIgnoreCase.Equals(x, "--compact")))
                                  || !System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            
            if (useCompactMode)
            {
                using (new CompactUI())
                {
                }
            }
            else
            {
#if WINDOWS
                // Run in WinForms mode by default on Windows
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
#else
                // Fallback to compact mode on non-Windows platforms
                using (new CompactUI())
                {
                }
#endif
            }
        }
    }
}