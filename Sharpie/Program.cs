using System;
using System.Diagnostics;
namespace Sharpie
{
    class Program
    {
        static void Main(string[] args)
        {
            
            int exType = 0;
            if(args.Length > 0)
            {
                exType = int.Parse(args[0]);
            }

            switch (exType)
            {
                case 0:
                    justTestArrayColumRowO();
                    break;
                case 1:
                    break;
                case 2:
                    break;
                default:
                    Console.Error.WriteLine("Switch-Case of type " + exType + " not implemented!\n");
                    break;
            }
        }






        static void justTestArrayColumRowO()
        {
            const int size = 20000;

            int[,] arr = new int[size,size];
            if (fillArray(arr, size))
            {
                testXYAccess(arr, size);
                testYXAccess(arr, size);
                testBkwXYAccess(arr, size);
                testBkwYXAccess(arr, size);
            }

        }

        static bool fillArray(int[,] arr, int size)
        {
            Random rnd = new Random(100);
            try
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        arr[i, j] = rnd.Next() % 50;
                    }
                }
            }
            catch(Exception e)
            {
                return false;
            }

            Console.WriteLine("Filled Array with "+size*size+" values\n");
            return true;
        }

        static void testXYAccess(int[,] arr, int size)
        {
            int counter = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for(int i =0;i< size; i++)
            {
                for(int j =0; j < size; j++)
                {
                    if (arr[i, j] == 12) counter++;
                }
            }

            stopwatch.Stop();
            Console.WriteLine("ElapsedTime for X->Y->Access: " + stopwatch.ElapsedMilliseconds+" ms. found "+counter+" hits \n");

        }        
        static void testYXAccess(int[,] arr, int size)
        {
            int counter = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for(int i =0;i< size; i++)
            {
                for(int j =0; j < size; j++)
                {
                    if (arr[j, i] == 12) counter++;
                }
            }
            stopwatch.Stop();
            Console.WriteLine("ElapsedTime for Y->X->Access: " + stopwatch.ElapsedMilliseconds+" ms. found "+counter+" hits \n");

        }        
        static void testBkwYXAccess(int[,] arr, int size)
        {
            int counter = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for(int i =0;i< size; i++)
            {
                for(int j =0; j < size; j++)
                {
                    if (arr[size-(j+1), size-(i+1)] == 12) counter++;
                }
            }
            stopwatch.Stop();
            Console.WriteLine("ElapsedTime for backwards Y->X->Access: " + stopwatch.ElapsedMilliseconds+" ms. found "+counter+" hits \n");

        }    
        static void testBkwXYAccess(int[,] arr, int size)
        {
            int counter = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for(int i =0;i< size; i++)
            {
                for(int j =0; j < size; j++)
                {
                    if (arr[size-(i+1), size-(j+1)] == 12) counter++;
                }
            }
            stopwatch.Stop();
            Console.WriteLine("ElapsedTime backwards X->Y->Access: " + stopwatch.ElapsedMilliseconds+" ms. found "+counter+" hits \n");

        }
    }
}
