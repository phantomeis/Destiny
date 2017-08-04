﻿using Destiny.Data;
using Destiny.IO;
using Destiny.Maple.Data;
using Destiny.Network;
using Destiny.Utility;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Destiny
{
    internal static class Application
    {
        public static string LaunchPath
        {
            get
            {
                return Directory.GetCurrentDirectory() + @"\";
            }
        }

        public static string ExecutablePath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";
            }
        }

        private static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].ToLower() == "setup" || !File.Exists(Application.ExecutablePath + "Configuration.ini"))
            {
                Setup.Run();
            }

            Log.Entitle("Destiny");

            try
            {
                Settings.Initialize();

                Database.Test();
                Database.Analyze(true);

                DataProvider.Initialize();

                MasterServer.Start();

#if DEBUG
                string linkPath = Path.Combine(Application.ExecutablePath, "LaunchClient.lnk");
                if (File.Exists(linkPath))
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = linkPath;
                    proc.Start();
                }
#endif
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            while (MasterServer.IsAlive)
            {
                // TODO: Implement a CLI of some sort.

                Console.Read();
            }

            Log.Inform("Press any key to quit.");

            Console.Read();
        }

        public static string ToCamel(this string value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
        }

        public static bool IsAlphaNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            foreach (char c in value)
            {
                if (!char.IsLetter(c) && !char.IsNumber(c))
                {
                    return false;
                }
            }

            return true;
        }

        public static string ClearFormatters(this string value)
        {
            return value.Replace("{", "{{").Replace("}", "}}");
        }
    }
}