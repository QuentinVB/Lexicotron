using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Lexicotron.Core
{
    public class FileReader
    {
        /// <summary>
        /// Read a text file from path and count the distincts words in the given article
        /// </summary>
        /// <param name="article">the given article</param>
        /// <param name="filename">path of file</param>
        public static void ProcessFile(Article article, string filename)
        {
            Console.WriteLine(filename);
            StringBuilder stringBuilder = new StringBuilder();

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filename,Encoding.UTF8))//path + "\\sample.txt")
                {

                    char c = (char)sr.Read();
                    while (c != (char)65535)
                    {     
                        //strip ponctuation " " . , : ; - ! ?
                        if (char.IsWhiteSpace(c) || !char.IsLetterOrDigit(c))
                        {
                            if(stringBuilder.Length>0)
                            {
                                string word = stringBuilder.ToString();

                                //Console.WriteLine(word);
                                article.AddWord(word.ToLower());
                                stringBuilder.Clear();
                            }    
                        }
                        else
                        {
                            stringBuilder.Append(c);
                        }                      
                        c = (char)sr.Read();
                    }


                    /*
                    sr.Read
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    //String line = await sr.ReadToEndAsync();
                    Console.WriteLine(line);
                    */
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
