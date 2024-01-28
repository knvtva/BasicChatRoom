using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BasicChatRoomServer
{
    class Program
    {
        static Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();

        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                // Defines the server port as 9001 int
                int server_port = 9001;
                // Defines the server address
                IPAddress localAddress = IPAddress.Parse("127.0.0.1");

                /* Create & Start the TCP Server hosted on the address
                127.0.0.1:9001 and log that the server is listening on that port*/
                server = new TcpListener(localAddress, server_port);
                server.Start();

                Console.WriteLine("Server is listening on port " + server_port);

                while (true)
                {
                    // Accept TCP Client connections
                    TcpClient client = server.AcceptTcpClient();

                    // Create a new thread for the client and start it
                    Thread thread = new Thread(() => HandleClient(client));
                    thread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                server?.Stop();
            }
        }

        static void HandleClient(TcpClient client)
        {
            string IDClient = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();
                
            clients.Add(IDClient, client);
            NetworkStream stream = client.GetStream();

            Byte[] data;
            int PreviousBytes;

            while (true)
            {
                data = new byte[1024];
                PreviousBytes = 0;

                try
                {
                    PreviousBytes = stream.Read(data, 0, data.Length);
                }
                catch
                {
                    break;
                }

                if (PreviousBytes == 0)
                {
                    break;
                }

                string recievedData = Encoding.ASCII.GetString(data, 0, PreviousBytes);
                Console.WriteLine($"Recieved data from {IDClient}: {recievedData}");
            }

            Console.WriteLine($"{IDClient} disconnected.");
            clients.Remove(IDClient);
            client.Close();
        }
    }
}