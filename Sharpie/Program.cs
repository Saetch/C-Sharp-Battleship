using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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



        static void TCPServer()
        {
            {
                TcpListener server = null;
                bool run = true;
                try
                {
                    // Set the TcpListener on port 13000.
                    Int32 port = 13000;
                    IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                    // TcpListener server = new TcpListener(port);
                    server = new TcpListener(localAddr, port);

                    // Start listening for client requests.
                    server.Start();

                    // Buffer for reading data
                    Byte[] bytes = new Byte[256];
                    String data = null;

                    // Enter the listening loop.
                    while (run)
                    {
                        Console.Write("Waiting for a connection... ");

                        // Perform a blocking call to accept requests.
                        // You could also use server.AcceptSocket() here.
                        TcpClient client = server.AcceptTcpClient();
                        Console.WriteLine("Connected!");

                        data = null;

                        // Get a stream object for reading and writing
                        NetworkStream stream = client.GetStream();

                        int messageLength;

                        // Loop to receive all the data sent by the client.
                        while ((messageLength = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, messageLength);
                            Console.WriteLine("Received: {0}", data);

                            // Process the data sent by the client.
                            data = data;

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("Sent: {0}", data);
                        }
                        Console.WriteLine(data+" received");
                        // Shutdown and end connection
                        client.Close();
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                }
                finally
                {
                    // Stop listening for new clients.
                    server.Stop();
                }

                Console.WriteLine("\nHit enter to continue...");
                Console.Read();
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
                Console.WriteLine(e.Message);
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
