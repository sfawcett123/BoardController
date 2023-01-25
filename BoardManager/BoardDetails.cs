// ***********************************************************************
// Assembly         : BoardManager
// Author           : steve
// Created          : 12-30-2022
//
// Last Modified By : steve
// Last Modified On : 01-17-2023
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
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace BoardManager
{
    /// <summary>
    /// Information about a connected board
    /// </summary>
    public sealed class BoardDetails : IDisposable
    {
        #region constants
        public const int BASEPORT = 9000;
        #endregion

        #region private variables     
        private readonly TcpServer _tcpServer;
        private Timer? _timer;
        #endregion

        #region public variables
        /// <summary>
        /// Board Name
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        public IOList InputData { get; internal set; }
        public IOList OutputData { get; internal set; }
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
          
        /// <summary>
        /// Sample Rate
        /// </summary>
        /// <value>Sample rate in seconds</value>
        public int Pulse { get; private set; } = 10;

        /// <summary>
        /// Indicates if board is Internal.
        /// </summary>
        /// <value><c>true</c> if board is internal otherwise, <c>false</c>.</value>
        public bool BoardInternal { get; set; } = false;

        /// <summary>
        /// Gets the connected address.
        /// </summary>
        /// <value>The connected address.</value>
        public string ConnectedAddress { get; internal set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">if set to <c>true</c> [start].</param>
        public BoardDetails( bool start=true )
        {
            Name = "Unknown";
            _tcpServer = new TcpServer(BASEPORT);
            ConnectedAddress = "Unknown";
            InputData = new();
            OutputData = new();

            if (start) Start();

        }
        #endregion Constructor

        #region public methods
        /// <summary>
        /// Starts a background process which will read/write data from/to the boards assigned Port.
        /// </summary>
        public void Start()
        {
            _timer = new Timer(ProcessBoard, null, TimeSpan.Zero, TimeSpan.FromSeconds(Rate));
            this.Timeout = 0;
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetPort()
        {
            return _tcpServer.Port.ToString();
        }

        /// <summary>
        /// Gets the ip address.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetIPAddress()
        {
            return _tcpServer.Address.MapToIPv4().ToString();
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

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> _serial = new() { { "name"      , Name            },
                                                         { "ip_address", GetIPAddress()  },
                                                         { "Port"      , GetPort()       },
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
            if (OutputData != null)
            {
                _tcpServer.WriteData(OutputData.Serialize());
            }

            string json_data = _tcpServer.ReadData();

            if (json_data.Length > 0 )
            {
                try
                {
                    var newdata = JsonSerializer.Deserialize<Dictionary<string, string>>(json_data);
                    foreach( var data in newdata )
                    {
                        InputData.AddUpdate(data);
                    }
                }
                catch {
                    Console.WriteLine($"Invalid Json {json_data}");
                }                
            }

            // If connected reset timeout, otherwise decrease counter.
            if( _tcpServer.Connection == ConnectState.Connected ) {
                this.Timeout = 0;
            }
            else
            {              
                 this.Timeout++;
            }

        }

        #endregion
    }
}