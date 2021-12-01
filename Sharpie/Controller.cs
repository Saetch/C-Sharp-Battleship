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
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

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




            throw new NotImplementedException();
        }



        private void CommunicationLoopS(NetworkStream stream)
        {

            Byte[] bytes = new Byte[256];
            String data;
            int messageLength;
            Random r = new Random();
            ushort rnd = (ushort)(r.Next() % 2);

            SendMessage(stream,"B_INIT:"+Model.Width+":"+Model.Height);

            SendMessage(stream, "B_FORCETURN:" + Model.PlayersTurn);


            // Loop to receive all the data sent by the client.
            while (Model.Active && (messageLength = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, messageLength);
                Console.WriteLine("Received: {0}", data);



                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                stream.Write(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0}", data);

            }
        }

        internal void StartGameClient(IPAddress targetAddress)
        {

        }

        private void CommunicationLoopC()
        {

        }

        private static void SendMessage(NetworkStream stream, String msg)
        {
            Console.WriteLine("Sending ... " + msg);

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
