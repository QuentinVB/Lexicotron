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
    /// <summary>
    /// Make the property non printable with excel
    /// </summary>
    public class NonPrintable : System.Attribute
    {

        public NonPrintable()
        {

        }
    }
}
