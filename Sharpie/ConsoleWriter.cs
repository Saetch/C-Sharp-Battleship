using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpie
{
    public class ConsoleWriter
    {

        public String Headline = "";

        public String TailLine { get; internal set; } = "";

        public bool printToConsole(Model m)
        {
            if (m.Active )
            {
                
                String str = "";
                for(int i = 0; i < m.Width; i++)
                {
                    str += "--";
                }
                Console.Clear();
                Console.WriteLine(Headline);
                Console.WriteLine(str);


                ConsoleWriter.PrintField(m,1);

                Console.WriteLine(str);

                ConsoleWriter.PrintField(m, 0);

                if(m.Status != 2 || m.PlayersTurn == 0)Console.WriteLine(TailLine);
                else
                {
                    Console.WriteLine("Waiting for your Turn ...");
                }


                return true;
            }
            else
            {
                return false;
            }
        }
        public static void PrintField(Model m, int playerNum)
        {
            short val = 0;
            Console.ResetColor();
            Console.Write("\n");
            short currentColorVal ;
            for(int j = 0; j < m.Height; j++)
            {
                currentColorVal = SetConsoleColorFromVal(val);
                for (int i = 0; i < m.Width; i++)
                {
                    val = (short)m.getVal(i, j, playerNum);
                    if(val != currentColorVal)
                    {
                        currentColorVal = SetConsoleColorFromVal(val);
                    }
                    Console.Write("  ");


                }
                Console.ResetColor();
                Console.Write("\n");
                
            }

        }

        private static short SetConsoleColorFromVal( short value)
        {
            switch (value)
            {
                case 0:
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case 1:
                    Console.BackgroundColor = ConsoleColor.White;
                    break;
                case 2:
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    break;
                case 3:
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;
                case -2:
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    break;
                case -1:
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case -3:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case 6:
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    break;
                case 4:
                    Console.BackgroundColor = ConsoleColor.Gray;
                    break;
                default:
                    Console.WriteLine("Could not find color for value {0}", value);
                    break;

            }
            return value;
        }
    }
    
}
