using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lexicotron.Core
{
    public class Helpers
    {
        public static string GetExecutingDirectoryPath()
        {
            return Environment.CurrentDirectory;
            /*
            return Path.GetDirectoryName(
                new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath
                );
                */
        }
    }
}
