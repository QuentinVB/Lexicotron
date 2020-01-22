using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lexicotron.Core
{
    public class Helpers
    {
        public static string GetExecutingDirectoryName()
        {
            return System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        }
    }
}
