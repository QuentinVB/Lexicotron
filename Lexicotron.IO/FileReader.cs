using System;
using System.IO;

namespace Lexicotron.IO
{
    public class FileReader
    {
        public static void read(string path)//async
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(path+"\sample.txt"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    //String line = await sr.ReadToEndAsync();
                    Console.WriteLine(line);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
