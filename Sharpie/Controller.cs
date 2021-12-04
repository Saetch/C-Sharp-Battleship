using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Sharpie
{
    class Controller
    {
        public Model Model { get; private set; }

        public Controller(Model mod) => Model = mod;

        internal void StartGameServer()
        {

            TcpListener server = null;
            bool run = true;
            try
            {
                Int32 port = 1024;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                Console.WriteLine("Listening for IPv4 or IPv6 address? [4/6]");
                {
                    String inp;
                    while( true){
                        inp = Console.ReadLine();
                        if(String.Equals(inp, "4"))
                        {
                            server = new TcpListener(IPAddress.Any, port);
                            break;
                        }
                        if(String.Equals(inp, "6"))
                        {
                            server = new TcpListener(IPAddress.IPv6Any, port);
                            break;
                        }
                    }
                    
                }


               


                server.Start();

                // Enter the listening loop.
                while (run)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");


                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    CommunicationLoopS(stream);

                    //TODO, correct this for multiple games without closing the socket
                    run = false;
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



            Console.ReadKey(true);
        }



        private void CommunicationLoopS(NetworkStream stream)
        {

            Byte[] bytes = new Byte[256];
            String data;
            Random r = new Random();
            ushort rnd = (ushort)(r.Next() % 2);

            SendMessage(stream,"B_INIT:"+Model.Width+":"+Model.Height);

            SendMessage(stream, "B_FORCETURN:" + Model.PlayersTurn);


            ConsoleWriter cnsl = new();
            cnsl.Headline = "Place your ships!";
            cnsl.TailLine = "[W/A/S/D/Arrow keys] Move Ship  [Space] Change direction  [Enter] Place Ship";
            Model.Activate();

            while (Console.KeyAvailable)
            {
                Console.ReadKey(false);
            }
            cnsl.printToConsole(Model);

            while (Model.Status == (int)StatusEnum.SettingUp)
            {

                data = Console.ReadKey(true).Key.ToString();
                if (ProcessInput(data.ToLower()))
                {
                    cnsl.printToConsole(Model);
                }
            }


            cnsl.printToConsole(Model);
            Console.WriteLine("Waiting for your opponent to finish setting up ...");

            SendMessage(stream, "B_INITDONE_SYNC");

            data = GetMessage(stream);

            if (!String.Equals(data, "B_INITDONE_SYNC"))
            {
                throw new InvalidOperationException();
            }
            cnsl.Headline = "Place your ships!";
            cnsl.TailLine = "[W/A/S/D/Arrow keys] Move Ship  [Space] Change direction  [Enter] Place Ship";
            cnsl.printToConsole(Model);

            GameLoop(cnsl, data, stream);


            
        }


        private void GameLoop(ConsoleWriter cnsl, String data, NetworkStream stream)
        {
            // Loop to receive all the data sent by the client.
            while (Model.Status == (int)StatusEnum.Playing)
            {
                if (Model.PlayersTurn == 0) Model.StartFight();

                while (Model.PlayersTurn == 0)
                {
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                    cnsl.printToConsole(Model);

                    data = Console.ReadKey(true).Key.ToString();
                    if (ProcessInput(data.ToLower()))
                    {
                        cnsl.printToConsole(Model);
                    }

                    if (Model.PlayersTurn == 1)
                    {
                        Console.WriteLine("Sending ... " + "B_HITCOORD:" + Model.ObjectX + ":" + Model.ObjectY);
                        SendMessage(stream, "B_HITCOORD:" + Model.ObjectX + ":" + Model.ObjectY);
                        data = GetMessage(stream);
                        if (String.Equals(data.Split(":")[1], "HIT"))
                        {
                            Model.Hit();
                            if (String.Equals(data.Split(":")[2], "WON"))
                            {
                                cnsl.TailLine = "WON!";
                                cnsl.Headline = "YOU WON!";
                                Model.Won();
                            }
                        }
                        else
                        {
                            Model.Miss();
                        }
                        cnsl.printToConsole(Model);
                    }

                }

                data = GetMessage(stream);
                if (data.StartsWith("B_HITCOORD:"))
                {
                    if (Model.CheckForHit(int.Parse(data.Split(":")[1]), int.Parse(data.Split(":")[2])))
                    {
                        String message = "B_HITCOORDANSWER:HIT";
                        if (!Model.GotHit())
                        {
                            message += ":WON";
                        }
                        else
                        {
                            message += ":ONGOING";
                        }
                        SendMessage(stream, message);


                    }
                    else
                    {
                        SendMessage(stream, "B_HITCOORDANSWER:MISS");
                    }
                }

            }

            if(Model.Hitpoints == 0)
            {
                cnsl.Headline = "You lost ...";
                cnsl.TailLine = "Your fleet got destroyed!";
            }
            cnsl.printToConsole(Model);
        }

        private bool ProcessInput(string data)
        {
            switch (data)
            {
                case "w":

                case "uparrow":
                    Model.Up();
                    break;
                case "a":
                case "leftarrow":
                    Model.Left();
                    break;
                case "s":
                case "downarrow":
                    Model.Down();
                    break;
                case "d":
                case "rightarrow":

                    Model.Right();
                    break;

                case "spacebar":
                    return Model.Space();

                case "enter":
                    Model.Enter();
                    break;
                default:
                    return false;
            }
            return true;
        }

        internal void StartGameClient(IPAddress targetAddress)
        {
            Console.Write("Connecting ... ");
            TcpClient cl = new TcpClient(targetAddress.ToString(), 1024);

            //cl = new TcpClient(AddressFamily.InterNetworkV6);
            //cl.Connect(targetAddress.ToString(), 13000);
            Console.Write("connected!\n");

            NetworkStream stream = cl.GetStream();

            CommunicationLoopC(stream);

            cl.Close();
            stream.Close();
        }


        private void CommunicationLoopC(NetworkStream stream)
        {
            Byte[] bytes = new Byte[256];
            String data;

            data = GetMessage(stream);
            Model.ForceWidthHeight(int.Parse(data.Split(":")[1]), int.Parse(data.Split(":")[2]));
            Model.CreateFields();
            data = GetMessage(stream);
            Model.ForceTurn((int.Parse(data.Split(":")[1])+1)%2);
            Console.WriteLine("Set Turn to: {0}", Model.PlayersTurn);

            ConsoleWriter cnsl = new();
            cnsl.Headline = "Place your ships!";
            cnsl.TailLine = "[W/A/S/D/Arrow keys] Move Ship  [Space] Change direction  [Enter] Place Ship";
            while (Console.KeyAvailable)
            {
                Console.ReadKey(false);
            }
            cnsl.printToConsole(Model);

            while (Model.Status == (int)StatusEnum.SettingUp)
            {

                data = Console.ReadKey(true).Key.ToString();
                if (ProcessInput(data.ToLower()))
                {
                    cnsl.printToConsole(Model);
                }
            }

            cnsl.printToConsole(Model);
            Console.WriteLine("Waiting for your opponent to finish setting up ...");

            SendMessage(stream, "B_INITDONE_SYNC");

            data = GetMessage(stream);

            if(!String.Equals(data,"B_INITDONE_SYNC"))
            {
                throw new InvalidOperationException();
            }
            cnsl.Headline = "Hit the enemy Ships!";
            cnsl.TailLine = "[W/A/S/D/Arrow keys] Move Target  [Enter] Attack Target";
            cnsl.printToConsole(Model);


            GameLoop(cnsl, data, stream);

        }



        private static void SendMessage(NetworkStream stream, String msg)
        {
            byte [] bytes = System.Text.Encoding.ASCII.GetBytes(msg);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static String GetMessage(NetworkStream stream)
        {
            byte[] bytes = new byte[256];
            int len = stream.Read(bytes, 0, bytes.Length);
            if (len == 0) return "";
            return System.Text.Encoding.ASCII.GetString(bytes, 0, len);
        }
    }

}
