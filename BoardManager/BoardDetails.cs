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

namespace BoardManager
{
    /// <summary>
    /// Information about a connected board
    /// </summary>
    public sealed class BoardDetails : IDisposable
    {
        #region constants
        /// <summary>
        /// The baseport
        /// </summary>
        public const int BASEPORT = 9000;
        #endregion

        #region private variables     
        /// <summary>
        /// The TCP listener
        /// </summary>
        private readonly TcpServer tcpServer;
        /// <summary>
        /// The timer
        /// </summary>
        private Timer? _timer;
        #endregion

        #region public variables
        /// <summary>
        /// Board Name
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
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
        /// List of data from Flight Simulator the board requires.
        /// </summary>
        /// <value>The output data.</value>
        public Dictionary<string, string>? OutputData { get; internal set; }
        /// <summary>
        /// Gets the input data.
        /// </summary>
        /// <value>The input data.</value>
        public Dictionary<string, string>? InputData { get; internal set; }

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">if set to <c>true</c> [start].</param>
        public BoardDetails( bool start=true )
        {
            Name = "Unknown";
            tcpServer = new TcpServer(BASEPORT);
            ConnectedAddress = "Unknown";

            if (start) Start();

        }

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
            return tcpServer.Port.ToString();
        }

        /// <summary>
        /// Gets the ip address.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetIPAddress()
        {
            return tcpServer.Address.MapToIPv4().ToString();
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
                tcpServer.WriteData(OutputData.Serialize());
            }

            string json_data = tcpServer.ReadData();

            if (json_data.Length > 0 )
            {
                try
                {
                    var newdata = JsonSerializer.Deserialize<Dictionary<string, string>>(json_data);
                    if (newdata != null)
                    {

                        if (InputData != null)
                        {
                            InputData = InputData.MergeLeft(newdata);
                        }
                        else
                        {
                            InputData = newdata;
                        }
                    }
                    
                }
                catch {
                    Console.WriteLine($"Invalid Json {json_data}");
                }                
            }

            // If connected reset timeout, otherwise decrease counter.
            if( tcpServer.Connection == ConnectState.Connected ) {
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