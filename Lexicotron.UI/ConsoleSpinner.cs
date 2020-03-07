using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lexicotron.UI.ConsoleHelper
{
    public class ConsoleSpinner
    {
        int counter;
        public ConsoleSpinner()
        {
        }

        public void Turn()
        {
            counter++;
            
            switch (counter % 4)
            {
                case 0: Console.Write("/"); counter = 0; break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }
            Thread.Sleep(100);
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
    }
}
