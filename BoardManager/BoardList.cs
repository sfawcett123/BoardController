// ***********************************************************************
// Assembly         : BoardManager
// Author           : steve
// Created          : 12-30-2022
//
// Last Modified By : steve
// Last Modified On : 12-30-2022
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
                IPAddress = _ip_address,
                Rate = 1,
                OS = _board.OperatingSystem,
                OutputData = _board.Outputs?.ToDictionary(keySelector: m => m, elementSelector: m => ""),
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
        public void RemoveTimedOut( int timeout = BoardDetails.TIMEOUT )
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
                b.OutputData = fs_data;
            }
        }

        /// <summary>
        /// Gets all output data.
        /// </summary>
        /// <returns>A dictionary of all outputs from every registered board.</returns>
        public Dictionary<string, string> GetAllOutputData()
        {
            Dictionary<string, string> all_data = new();

            foreach (BoardDetails b in boards.Where(x => x is not null ))
            {
                if (b.OutputData != null)
                {
                    all_data = all_data.MergeLeft(b.OutputData);
                }
            }

            return all_data;
        }

        public Dictionary<string, string> GetAllInputData()
        {
            Dictionary<string, string> all_data = new();

            foreach (BoardDetails b in boards.Where(x => x is not null))
            {
                if (b.InputData != null)
                {
                    all_data = all_data.MergeLeft(b.InputData);
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
