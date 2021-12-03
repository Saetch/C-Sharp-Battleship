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

        public bool printToConsole(Model m)
        {
            if (m.Active)
            {
                String str = "";
                for(int i = 0; i < m.Width; i++)
                {
                    str += "-";
                }
                Console.WriteLine(Headline);
                Console.WriteLine(str);


                ConsoleWriter.PrintField(m,1);



                return true;
            }
            else
            {
                return false;
            }
        }
        public static void PrintField(Model m, int playerNum)
        {
            short val;
            short currentColorVal = SetConsoleColorFromVal(0);
            for(int j = 0; j < m.Height; j++)
            {
                for(int i = 0; i < m.Width; i++)
                {
                    val = (short)m.getVal(i, j, playerNum);
                    if(val != currentColorVal)
                    {
                        currentColorVal = SetConsoleColorFromVal(val);
                    }
                }
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
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case -1:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case -3:
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    break;
                default:
                    Console.WriteLine("Could not find color for value {0}", value);
                    break;

            }
            return value;
        }
    }
    
}
