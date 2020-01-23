﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lexicotron.Core
{
    public class Process
    {
        public static List<Article> ProcessDirectory(string path)//async
        {
            
            string[] fileEntries = Directory.GetFiles(path);

            List<Article> _articles = new List<Article>(fileEntries.Length);

            foreach (string filePath in fileEntries)
            {
                if (filePath.EndsWith(".txt"))
                {
                    //TODO : strip path + use array instead of list
                    Article article = new Article(GetNameOnlyFromPath(filePath));
                    _articles.Add(article);
                    FileReader.ProcessFile(article, filePath);
                }
                    
            }

            return _articles;
        }

        public static string GetNameOnlyFromPath(string path)
        {
            int i = path.Length-1;
            do
            {
                if(path[i]=='\\')
                {
                    return path[i..]; //substitute path.Substring(i, path.Length - i); :)
                }

                i--;
            } while (i > 0);
            throw new ArgumentException("Not a path provided");
        }
    }
}