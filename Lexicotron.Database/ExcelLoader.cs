using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Lexicotron.Database
{
    public class ExcelLoader
    {
        public static void OpenExcelFile(string path)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            string str;
            int rCnt;
            int cCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            rw = range.Rows.Count;
            cl = range.Columns.Count;


            for (rCnt = 1; rCnt <= rw; rCnt++)
            {
                for (cCnt = 1; cCnt <= cl; cCnt++)
                {
                    str = (string)(range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                }
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
        }

        public static (Dictionary<string,string[]>,string[], Dictionary<string, string[]>) LoadLexicalField(string path)
        {
            //TODO: make it cleaner
            //word->lexical fields
            Dictionary<string, string[]> _lexicalFieldAssociations = new Dictionary<string, string[]>();
            Dictionary<string, string[]> _lexicalFieldToWordsAssociations = new Dictionary<string, string[]>();
            List<string> lexicalFields= new List<string>();

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            //string str;
            int rCnt;
            //int cCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            rw = range.Rows.Count;
            cl = range.Columns.Count;


            for (rCnt = 1; rCnt <= rw; rCnt++)
            {              
                string lexicalField = (string)(range.Cells[rCnt, 1] as Excel.Range).Value2;
                lexicalFields.Add(lexicalField);

                string[] words = ((string)(range.Cells[rCnt, 2] as Excel.Range).Value2).Split(';');

                _lexicalFieldToWordsAssociations.Add(lexicalField, words);

                for (int i = 0; i < words.Length; i++)
                {
                    if (_lexicalFieldAssociations.ContainsKey(words[i]))
                    {
                        string[] updatedLexicalFieldOfWord = new string[_lexicalFieldAssociations[words[i]].Length+1];
                        
                        _lexicalFieldAssociations[words[i]].CopyTo(updatedLexicalFieldOfWord, 0);

                        updatedLexicalFieldOfWord[_lexicalFieldAssociations[words[i]].Length] = lexicalField;

                        _lexicalFieldAssociations[words[i]] = updatedLexicalFieldOfWord;
                    }
                    else
                    {
                        _lexicalFieldAssociations.Add(words[i], new string[1] { lexicalField });
                    }
                }

            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            return (_lexicalFieldAssociations,lexicalFields.ToArray(), _lexicalFieldToWordsAssociations);
        }
    }
}
