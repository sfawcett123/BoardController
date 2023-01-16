using System;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace BoardManager
{
    class TcpServer
    {
        public int Port { get; }
        private readonly TcpListener server ;
        private TcpClient? client;
        private string last = string.Empty;

        private static readonly ManualResetEvent tcpClientConnected = new(false);
        public TcpServer(int port)
        {;
            this.Port = port;
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            DoBeginAcceptTcpClient();
        }

        public static int  GetNextAvailablePort(int baseport )
        {
            IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = iPGlobalProperties.GetActiveTcpConnections();
            bool isAvailable;
            int _port;

            for (_port = baseport; _port < baseport + 1000; _port++)
            {
                isAvailable = true;

                foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
                {
                    if (tcpi.LocalEndPoint.Port == _port)
                    {
                        isAvailable = false;
                        break;
                    }
                }

                if (isAvailable)
                {
                    return _port;
                }
            }

            return -1;
        }

        public void WriteData( string msg )
        {
            if (client != null)
            {
                if (last != msg)
                {
                    Console.WriteLine($"Writing to Port {Port} -> {msg}");

                    WriteLine(client, msg);

                    last = msg;
                }
            }
        }
        private void DoBeginAcceptTcpClient()
        {
            tcpClientConnected.Reset();

            Console.WriteLine("Waiting for a connection...");

            server.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), server);

            tcpClientConnected.WaitOne();
        }

        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            if( ar == null) { return; }
            if( ar.AsyncState == null) { return; }

            TcpListener listener = (TcpListener)ar.AsyncState;

            client = listener.EndAcceptTcpClient(ar);

            Console.WriteLine("Client connected completed");

            tcpClientConnected.Set();
        }

        private static void WriteLine(TcpClient client , string _str)
        {
            if (client == null) return;

            try
            {
                NetworkStream stream = client.GetStream();
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(_str);
                stream.Write(msg, 0, msg.Length);
            }
            catch
            {
                Console.WriteLine("Connection lost");
                client?.Close();
            }
        }
    }
}
