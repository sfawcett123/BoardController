using System;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace BoardManager
{
    internal enum ConnectState
    {
        Connected,
        Disconnected,
        Pending,
        Allocating,
    }
    class TcpServer
    {
        public int Port { get; }
        public ConnectState Connection { get; internal set; }
        public IPAddress  Address { get; internal set; }

        private readonly TcpListener server ;
        private TcpClient? client;
        private string last = string.Empty;

        private static readonly ManualResetEvent tcpClientConnected = new(false);
        public TcpServer( int baseport )
        {
            Connection = ConnectState.Allocating;
            this.Address = IPAddress.Any;
            this.Port = baseport;

            while (Connection == ConnectState.Allocating)
            {
                try
                {
                    this.Port = GetNextAvailablePort(baseport);
                    server = new TcpListener(this.Address, Port);
                    server.Start();
                    DoBeginAcceptTcpClient();
                    Connection = ConnectState.Disconnected;
                }
                catch
                {
                    baseport++;
                }
            }
            Console.WriteLine($"Listening on port {Port}");
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
                    Console.WriteLine($"Listening on port {_port}");
                    return _port;
                }
            }

            return -1;
        }

        public void WriteData( string msg )
        {
            // If we loose connection lets restart listener to see if it comes back
            if (Connection == ConnectState.Disconnected )
            {
                DoBeginAcceptTcpClient();
            }

            // We are  connected lets write out data
            if (client != null && Connection == ConnectState.Connected)
            {
                // Only write message out if it differs
                if (last != msg)
                {
                    WriteLine(msg);

                    last = msg;
                }
            }
        }

        public string ReadData()
        {
            // If we loose connection lets restart listener to see if it comes back
            if (Connection == ConnectState.Disconnected)
            {
                DoBeginAcceptTcpClient();
            }

            // We are not connected so no need to read data
            if (client == null || Connection != ConnectState.Connected) return "";

            return ReadLine();
        }
        private void DoBeginAcceptTcpClient()
        {
            // If we are connected or pending then we don't need to proceed.
            if (Connection != ConnectState.Disconnected) return;

            tcpClientConnected.Reset();
            Connection = ConnectState.Pending ;

            // Wait for a connection
            server.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), server);

            tcpClientConnected.WaitOne();
        }

        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            if( ar == null) { return; }
            if( ar.AsyncState == null) { return; }

            // We have a connection. Create a client 

            TcpListener listener = (TcpListener)ar.AsyncState;

            client = listener.EndAcceptTcpClient(ar);
            
            Connection = ConnectState.Connected;

            tcpClientConnected.Set();
        }

        private void WriteLine( string _str)
        {
            if (client == null) return;

            try
            {
                
                NetworkStream stream = client.GetStream();
                byte[] msg = Encoding.ASCII.GetBytes(_str);
                stream.Write(msg, 0, msg.Length);
                byte[] LINEFEED = Encoding.ASCII.GetBytes("\r");
                stream.Write( LINEFEED, 0, LINEFEED.Length );
            }
            catch
            {
                // We lost our connection, Flag the system so it can reset
                Connection = ConnectState.Disconnected;
                client?.Close();                     
            }
        }

        private string ReadLine()
        {
            if (client == null) return "";

            try
            {
                NetworkStream stream = client.GetStream();
                byte[] receiveBuffer = new byte[1024];
                int bytesReceived = stream.Read(receiveBuffer);
                string data = Encoding.UTF8.GetString(receiveBuffer.AsSpan(0, bytesReceived));
                return data;
            }
            catch
            {
                // We lost our connection, Flag the system so it can reset
                Connection = ConnectState.Disconnected;
                client?.Close();
            }

            return "";
        }
    }
}
