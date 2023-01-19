// ***********************************************************************
// Assembly         : BoardManager
// Author           : steve
// Created          : 01-16-2023
//
// Last Modified By : steve
// Last Modified On : 01-17-2023
// ***********************************************************************
// <copyright file="tcpServer.cs" company="BoardManager">
//     Steven Fawcett
// </copyright>
// <summary></summary>
// ***********************************************************************
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
    /// <summary>
    /// Enum ConnectState
    /// </summary>
    internal enum ConnectState
    {
        /// <summary>
        /// The connected
        /// </summary>
        Connected,
        /// <summary>
        /// The disconnected
        /// </summary>
        Disconnected,
        /// <summary>
        /// The pending
        /// </summary>
        Pending,
        /// <summary>
        /// The allocating
        /// </summary>
        Allocating,
    }
    /// <summary>
    /// Class TcpServer.
    /// </summary>
    class TcpServer
    {
        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; }
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public ConnectState Connection { get; internal set; }
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        public IPAddress  Address { get; internal set; }

        /// <summary>
        /// The server
        /// </summary>
        private readonly TcpListener server ;
        /// <summary>
        /// The client
        /// </summary>
        private TcpClient? client;
        /// <summary>
        /// The last
        /// </summary>
        private string last = string.Empty;

        /// <summary>
        /// The TCP client connected
        /// </summary>
        private static readonly ManualResetEvent tcpClientConnected = new(false);
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class.
        /// </summary>
        /// <param name="baseport">The baseport.</param>
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
        }

        /// <summary>
        /// Gets the next available port.
        /// </summary>
        /// <param name="baseport">The baseport.</param>
        /// <returns>System.Int32.</returns>
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

        /// <summary>
        /// Writes the data.
        /// </summary>
        /// <param name="msg">The MSG.</param>
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

        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <returns>System.String.</returns>
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
        /// <summary>
        /// Does the begin accept TCP client.
        /// </summary>
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

        /// <summary>
        /// Does the accept TCP client callback.
        /// </summary>
        /// <param name="ar">The ar.</param>
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

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="_str">The string.</param>
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

        /// <summary>
        /// Reads the line.
        /// </summary>
        /// <returns>System.String.</returns>
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
