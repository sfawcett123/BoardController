// ***********************************************************************
// Assembly         : BoardManager
// Author           : steve
// Created          : 12-30-2022
//
// Last Modified By : steve
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="BoardDetails.cs" company="BoardManager">
//     Steven Fawcett
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace BoardManager
{
    /// <summary>
    /// Information about a connected board
    /// </summary>
    public sealed class BoardDetails : IEquatable<BoardDetails?>, IDisposable
    {
        #region constants
        /// <summary>
        /// Timeout Period
        /// </summary>
        public const int TIMEOUT = 100;
        #endregion

        #region private variables
        /// <summary>
        /// The ip address
        /// </summary>
        private IPAddress? ip_address;
        /// <summary>
        /// The client
        /// </summary>
        private TcpClient? client;
        /// <summary>
        /// The TCP listener
        /// </summary>
        private TcpListener? tcpListener;
        /// <summary>
        /// The timer
        /// </summary>
        private Timer? _timer;
        /// <summary>
        /// The TCP client connected
        /// </summary>
        private static readonly ManualResetEvent tcpClientConnected = new(false);
        /// <summary>The baseport</summary>
        private readonly int baseport = 9000;
        /// <summary>
        /// The last string
        /// </summary>
        private string last_str = "";
        #endregion

        #region public variables
        /// <summary>
        /// Board Name
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// IP address of board
        /// </summary>
        /// <value>The ip address.</value>
        public string IPAddress
        {
            get => ip_address == null ? "Unknown" : ip_address.ToString();
            set => ip_address = System.Net.IPAddress.Parse(value);
        }
        /// <summary>
        /// Port number the board will communicate on
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; }
        /// <summary>
        /// Sample Rate
        /// </summary>
        /// <value>The rate.</value>
        public int Rate { get; set; } = 1;
        /// <summary>
        /// Operating system of the board
        /// </summary>
        /// <value>The os.</value>
        public string OS { get; set; } = "UNKNOWN";

        /// <summary>
        /// Should this be public?
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout { get; set; }

        public bool TimeStarted {  get { return _timer != null; } }
        /// <summary>
        /// Hash of Board
        /// </summary>
        /// <value>The hash.</value>
        public int Hash => GetHashCode();

        /// <summary>
        /// List of data from Flight Simulator the board requires.
        /// </summary>
        /// <value>The output data.</value>
        public Dictionary<string, string>? OutputData { get; internal set; }

        /// <summary>Sample Rate </summary>
        /// <value>Sample rate in seconds</value>
        public int Pulse { get; private set; } = 10;
        /// <summary>Indicates if board is Internal.</summary>
        /// <value>
        ///   <c>true</c> if board is internal otherwise, <c>false</c>.</value>
        public bool BoardInternal { get; set; } = false;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BoardDetails( bool start=true )
        {
            Name = "Unknown";
            if (start) Start();
        }

        #region public methods

        /// <summary>
        /// Starts TCP Connection to a board
        /// </summary>
        public void Start()
        {
            _timer = new Timer(ProcessBoard, null, TimeSpan.Zero, TimeSpan.FromSeconds(Rate));

            if (BoardInternal is true) return;

            IPAddress local_ip_address = GetIPAddress();

            tcpListener = GetNextAvailablePort(local_ip_address);
        }

        /// <summary>
        /// Override the ToString Method
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if ( tcpListener == null)
                return "Unknown";

            var x = tcpListener.LocalEndpoint.ToString();
            if ( x == null )
                return "Unknown";
            else
                return x.ToString();

        }

        /// <summary>
        /// Dispose of board
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Calculate HASH
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return tcpListener is null
                ? HashCode.Combine(ip_address, Port)
                : HashCode.Combine(ip_address, tcpListener.LocalEndpoint.ToString());
        }
        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BoardDetails? left, BoardDetails? right)
        {
            return EqualityComparer<BoardDetails>.Default.Equals(left, right);
        }
        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BoardDetails? left, BoardDetails? right)
        {
            return !(left == right);
        }
        /// <summary>
        /// Comparison method
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as BoardDetails);
        }
        /// <summary>
        /// Comparison method
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(BoardDetails? other)
        {
            return other is not null &&
                   Port == other.Port &&
                   ip_address == other.ip_address;
        }
        /// <summary>
        /// Converts to dictionary.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> _serial = new() { { "name"      , Name            },
                                                         { "ip_address", IPAddress       },
                                                         { "port"      , Port.ToString() },
                                                         { "os"        , OS              } };
            return _serial;
        }

        #endregion

        #region private methods
        /// <summary>
        /// Processes the board.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ProcessBoard(object? state)
        {
            Timeout++;

            if (tcpListener is null)
            {
                Start();
                DoBeginAcceptTcpClient();
            }
            else
            {
                if (!tcpListener.Pending())
                {
                    if (OutputData is not null)
                    {
                        WriteLine(OutputData.Serialize());
                    }
                }
                else
                {
                    DoBeginAcceptTcpClient();
                }
            }
        }


        /// <summary>
        /// Gets the ip address.
        /// </summary>
        /// <returns>IPAddress.</returns>
        private static IPAddress GetIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); 
            
            if( ipHostInfo is not null ) return ipHostInfo.AddressList[0];
            
            return System.Net.IPAddress.Parse("127.0.0.1");
        }

        /// <summary>
        /// Does the begin accept TCP client.
        /// </summary>
        private void DoBeginAcceptTcpClient()
        {
            _ = tcpClientConnected.Reset();

            if (tcpListener is not null)
            {
                _ = tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), tcpListener);
            }
            _ = tcpClientConnected.WaitOne();
        }
        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="_str">The string.</param>
        private void WriteLine(string _str)
        {
            if (client is null)
            {
                return;
            }

            if (client.Connected is false)
            {
                return;
            }

            if (Timeout % Pulse != 0 && _str == last_str)
            {
                return;
            }

            try
            {
                NetworkStream stream = client.GetStream();
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(_str);
                stream.Write(msg, 0, msg.Length);
                last_str = _str;
                Timeout = 0;

            }
            catch
            {
                client.Close();
            }
        }
        /// <summary>
        /// Does the accept TCP client callback.
        /// </summary>
        /// <param name="ar">The ar.</param>
        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            if (tcpListener is null)
            {
                return;
            }

            client = tcpListener.EndAcceptTcpClient(ar);

            Timeout = 0;

            _ = tcpClientConnected.Set();
        }
        #endregion

        #region private static methods
        /// <summary>
        /// Gets the next available port.
        /// </summary>
        /// <param name="local_ip_address">The local ip address.</param>
        /// <returns>System.Nullable&lt;TcpListener&gt;.</returns>
        private TcpListener? GetNextAvailablePort(IPAddress local_ip_address)
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
                    try
                    {
                        TcpListener _local = new(local_ip_address, _port);
                        _local.Start();
                        Port = _port;
                        return _local;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}