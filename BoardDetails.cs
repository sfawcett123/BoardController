using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BoardController
{
    /// <summary>
    /// Information about a connected board
    /// </summary>
    public class BoardDetails : IEquatable<BoardDetails?>, IDisposable
    {
        #region constants
        /// <summary>
        /// Timeout Period
        /// </summary>
        public const int TIMEOUT = 100;
        #endregion

        #region private variables
        private IPAddress? ip_address;
        private TcpClient? client;
        private TcpListener? tcpListener;
        private readonly Timer _timer;
        private static readonly ManualResetEvent tcpClientConnected = new(false);
        private readonly int baseport = 9000;
        private string last_str = "";
        #endregion

        #region public variables
        /// <summary>
        /// Board Name
        /// </summary>
        public string Name { get; set; } = "unknown";
        /// <summary>
        /// IP address of board
        /// </summary>
        public string IPAddress
        {
            get => ip_address == null ? "Unknown" : ip_address.ToString();
            set => ip_address = System.Net.IPAddress.Parse(value);
        }
        /// <summary>
        /// Port number the board will communicate on
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Sample Rate
        /// </summary>
        public int Rate { get; set; } = 1;
        /// <summary>
        /// Operating system of the board
        /// </summary>
        public string OS { get; set; } = "UNKNOWN";

        /// <summary>
        /// Should this be public?
        /// </summary>
        public int Timeout { get; private set; }

        /// <summary>
        /// Hask of Board
        /// </summary>
        public int Hash => GetHashCode();

        /// <summary>
        /// List of data from Flight Simulator the board requires.
        /// </summary>
        public Dictionary<string, string>? OutputData { get; internal set; }

        /// <summary>
        /// Tick rate?
        /// </summary>
        public int Pulse { get; private set; } = 10;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BoardDetails()
        {
            DoStart();
            _timer = new Timer(ProcessBoard, null, TimeSpan.Zero, TimeSpan.FromSeconds(Rate));
        }
        #region public methods
        /// <summary>
        /// Override the ToString Method
        /// </summary>
        /// <returns></returns>
        public override string? ToString()
        {
            return tcpListener is null ? "Unknown" : tcpListener.LocalEndpoint is null ? "Unknown" : tcpListener.LocalEndpoint.ToString();
        }
        /// <summary>
        /// Comparison method
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BoardDetails? other)
        {
            return other is not null &&
                   Port == other.Port &&
                   ip_address == other.ip_address;
        }
        /// <summary>
        ///  Dispose of board
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Calculate HASH
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return tcpListener is null
                ? HashCode.Combine(ip_address, Port)
                : HashCode.Combine(ip_address, tcpListener.LocalEndpoint.ToString());
        }
        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(BoardDetails? left, BoardDetails? right)
        {
            return EqualityComparer<BoardDetails>.Default.Equals(left, right);
        }
        /// <summary>
        /// Comaprison operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(BoardDetails? left, BoardDetails? right)
        {
            return !(left == right);
        }
        /// <summary>
        /// Comaprison method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as BoardDetails);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        private void ProcessBoard(object? state)
        {
            Timeout++;

            //Console.WriteLine("Processing board [{0}]", timeout );

            if (tcpListener is null)
            {
                DoStart();
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
        private void DoStart()
        {
            // TODO: Do not use hard coded IP address
            IPAddress local_ip_address = System.Net.IPAddress.Parse("192.168.1.134");
            tcpListener = GetNextAvailablePort(local_ip_address);
        }
        private void DoBeginAcceptTcpClient()
        {
            _ = tcpClientConnected.Reset();

            if (tcpListener is not null)
            {
                Console.WriteLine("Waiting for a connection from {0}:{1}", ip_address, Port);

                _ = tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), tcpListener);
            }
            _ = tcpClientConnected.WaitOne();
        }
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
                Console.WriteLine("Connection on {0}:{1} Lost", ip_address, Port);
                client.Close();
            }
        }
        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            if (tcpListener is null)
            {
                return;
            }

            client = tcpListener.EndAcceptTcpClient(ar);

            Timeout = 0;
            Console.WriteLine("Listening on {0} , Port {1} for TCP data",
                                System.Net.IPAddress.Parse(((IPEndPoint)tcpListener.LocalEndpoint).Address.ToString()),
                                ((IPEndPoint)tcpListener.LocalEndpoint).Port.ToString());

            //int i;
            //NetworkStream stream = client.GetStream();
            //byte[] bytes = new byte[client.ReceiveBufferSize];
            //while ((i = stream.Read(bytes, 0, client.ReceiveBufferSize)) != 0)
            //{
            // Translate data bytes to a ASCII string.
            // string data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            // TODO: Add code to handle incoming data
            //Console.WriteLine("Received: {0}", data);
            // } 


            _ = tcpClientConnected.Set();
        }
        #endregion

        #region private static methods
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
                        Console.WriteLine("Starting board on {0} port {1}", local_ip_address, _port);
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