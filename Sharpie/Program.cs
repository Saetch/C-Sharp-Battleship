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
                if(String.Equals(args[0], "-l"))
                {
                    exType = 1;
                }               
                if(String.Equals(args[0], "-c"))
                {
                    exType = 2;
                }
                
            }

            switch (exType)
            {
                case 0:
                    justTestArrayColumRowO();
                    break;
                case 1:
                    launchTCPServer();
                    break;
                case 2:
                    launchTCPClient();
                    break;
                default:
                    Console.Error.WriteLine("Switch-Case of type " + exType + " not implemented!\n");
                    break;
            }
        }

        static void launchTCPClient()
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = 13000;

                //THIS IS HARDCODED FOR LOCALHOST, UPDATE IF NEEDED

                TcpClient client = new TcpClient("127.0.0.1", port);

                Console.Write("Connecting to Server ... ");
                Console.WriteLine(" connected!");
                NetworkStream stream = client.GetStream();


                string message;
                bool run = true;
                Int32 bytes =0;
                while (run)
                {
                    message = Console.ReadLine();

                    if (message == "disconnect" || message == "end")
                    {
                        run = false;
                    }
                    // Translate the passed message into ASCII and store it as a Byte array.
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                    // Get a client stream for reading and writing.
                    //  Stream stream = client.GetStream();

                    try
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException is System.IO.IOException)
                        {
                            Console.WriteLine("Connection closed!");
                            run = false;
                        }
                    }
            // Send the message to the connected TcpServer.

            Console.WriteLine("Sent: {0}", message);

                    // Receive the TcpServer.response.

                    // Buffer to store the response bytes.
                    data = new Byte[256];

                    // String to store the response ASCII representation.
                    String responseData = String.Empty;
                    try { 
                    // Read the first batch of the TcpServer response bytes.
                    bytes = stream.Read(data, 0, data.Length);
                    }
                    catch(Exception e)
                    {
                        if (e.InnerException is System.IO.IOException)
                        {
                            Console.WriteLine("Connection closed!");
                            run = false;
                        }
                    }
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine("Received: {0}", responseData);
                }


                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }


        static void launchTCPServer()
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
                    Random rnd = new Random();
                    // Enter the listening loop.
                    while (run)
                    {
                        Console.Write("Waiting for a connection... ");
                        bool active = true;

                        // Perform a blocking call to accept requests.
                        // You could also use server.AcceptSocket() here.
                        TcpClient client = server.AcceptTcpClient();
                        Console.WriteLine("Connected!");

                        data = null;

                        // Get a stream object for reading and writing
                        NetworkStream stream = client.GetStream();

                        int messageLength;

                        // Loop to receive all the data sent by the client.
                        while (active && (messageLength = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, messageLength);
                            Console.WriteLine("Received: {0}", data);

                            // Process the data sent by the client.
                            data = data;

                            //randomize  string somewhat
                            string data2 = "";

                            for(int i =0; i < data.Length; i++)
                            {
                                data2 += data[rnd.Next() % data.Length];
                            }

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data2);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("Sent: {0}", data2);
                            if (data == "end")
                            {
                                run = active = false;
                            }
                            if(data == "disconnect")
                            {
                                active = false;
                            }
                        }

                        stream.Close();
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
