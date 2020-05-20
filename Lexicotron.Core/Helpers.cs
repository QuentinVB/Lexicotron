using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lexicotron.Core
{
    public static class Helpers
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

        /// <summary>
        /// Truncate a string if too long
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxChars"></param>
        /// <returns></returns>
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) ;
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
