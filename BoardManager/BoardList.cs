// ***********************************************************************
// Assembly         : BoardManager
// Author           : steve
// Created          : 12-30-2022
//
// Last Modified By : steve
// Last Modified On : 01-17-2023
// ***********************************************************************
// <copyright file="BoardList.cs" company="BoardManager">
//     Steven Fawcett
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.AspNetCore.Http;
using System.Collections.Immutable;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace BoardManager
{
    /// <summary>
    /// List of Boards
    /// </summary>
    public abstract class BoardList
    {

        public const int TIMEOUT = 100;


        /// <summary>
        /// The boards
        /// </summary>
        private readonly List<BoardDetails> boards = new();

        /// <summary>
        /// Adds the specified board.
        /// </summary>
        /// <param name="_board">The board.</param>
        /// <returns><br /></returns>
        public string Add(Board _board)
        {
            return Add(_board, null);
        }
        /// <summary>
        /// Add another board
        /// </summary>
        /// <param name="_board">The board.</param>
        /// <param name="_request">The request.</param>
        /// <returns>System.String.</returns>
        public string Add(Board _board, ConnectionInfo? _request)
        {
            string _ip_address = "127.0.0.1";

            if (_request is not null && _request.RemoteIpAddress is not null)
                     _ip_address = _request.RemoteIpAddress.ToString();

            BoardDetails _bd = new()
            {
                Name = _board.Name,
                BoardInternal = _ip_address == "127.0.0.1",
                ConnectedAddress = _ip_address,
                Rate = 1,
                OS = _board.OperatingSystem,
                OutputData = new(_board.Outputs)
            };
            
            if (!boards.Contains<BoardDetails>(_bd))
            {
                boards.Add(_bd);
            }

            return _bd.ToDictionary().Serialize();

        }
        /// <summary>
        /// Remove Timed out boards
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void RemoveTimedOut( int timeout = TIMEOUT )
        {
            foreach (BoardDetails b in boards.Where(s => s.Timeout > timeout).ToArray<BoardDetails>())
            {
                if (!b.BoardInternal)
                {
                    _ = boards.Remove(b);
                    b.Dispose();
                }
            }
        }
        /// <summary>
        /// Convert board data to JSON
        /// </summary>
        /// <returns>System.String.</returns>
        public string Serialize()
        {
            return JsonSerializer.Serialize(boards);
        }
        /// <summary>
        /// For each board set the output data
        /// </summary>
        /// <param name="fs_data">The fs data.</param>
        // TODO: Output data needs to be specific to each boards requirements
        // current implementation has all boards set to all data

        public void SetOutputData(Dictionary<string, string> fs_data)
        {
            foreach (BoardDetails b in boards)
            {
                b.OutputData = new( fs_data );
            }
        }

        /// <summary>
        /// Gets all output data.
        /// </summary>
        /// <returns>A dictionary of all outputs from every registered board.</returns>
        public Dictionary<string, string> GetAllOutputData()
        {
            Dictionary<string, string> all_data = new();

            foreach (BoardDetails b in boards.Where(x => x is not null))
            {
                System.Collections.IList list = b.OutputData;
                for (int i = 0; i < list.Count; i++)
                {
                    KeyValuePair<string, string> data = (KeyValuePair<string, string>)list[i];
                    all_data.AddUpdate(data);
                }
            }

            return all_data;
        }

        /// <summary>
        /// Gets all input data.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public Dictionary<string, string> GetAllInputData()
        {
            Dictionary<string, string> all_data = new();

            foreach (BoardDetails b in boards.Where(x => x is not null))
            {
                foreach( KeyValuePair<string,string>  data in b.InputData.ToKeyValuePair() )
                {
                    all_data.AddUpdate(data);
                }
            }

            return all_data;
        }

        public Dictionary<string, string> GetChangedInputData()
        {
            Dictionary<string, string> all_data = new();

            foreach (BoardDetails b in boards.Where(x => x is not null))
            {
                foreach (KeyValuePair<string, string> data in b.InputData.ChangedData())
                {
                    all_data.AddUpdate(data);
                }
            }

            return all_data;
        }

        /// <summary>
        /// Gets the boards.
        /// </summary>
        /// <returns><br /></returns>
        public List<BoardDetails> GetBoards()
        {
            return boards;
        }
    }
}
