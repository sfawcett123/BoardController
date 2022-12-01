using Microsoft.AspNetCore.Http;
using System.Collections.Immutable;
using System.Text.Json;


namespace BoardController
{
    /// <summary>
    /// List of Boards
    /// </summary>
    public abstract class BoardList
    {
        /// <summary>The boards</summary>
        private readonly List<BoardDetails> boards = new();

        /// <summary>
        /// Add another board
        /// </summary>
        /// <param name="_board"></param>
        /// <param name="_request"></param>
        /// <returns></returns>
        public string Add(Board _board, ConnectionInfo _request)
        {
            string _ip_address = "localhost";

            if (_request is not null)
            {
                if (_request.RemoteIpAddress is not null)
                {
                    _ip_address = _request.RemoteIpAddress.ToString();
                }
            }

            Console.WriteLine("Request from " + _ip_address + " Name = " + _board.Name);

            BoardDetails _bd = new()
            {
                Name = _board.Name,
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
        public void RemoveTimedOut()
        {
            foreach (BoardController.BoardDetails b in boards.Where(s => s.Timeout > BoardDetails.TIMEOUT).ToArray<BoardDetails>())
            {
                _ = boards.Remove(b);
                b.Dispose();
            }
        }
        /// <summary>
        /// Convert board data to JSON
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return JsonSerializer.Serialize(boards);
        }
        /// <summary>
        /// For each board set the output data
        /// </summary>
        /// <param name="fs_data"></param>
        // TODO: Output data needs to be specific to each boards requirements
        // current implemtation has all boards set to all data

        public void SetOutputData(Dictionary<string, string> fs_data)
        {
            foreach (BoardController.BoardDetails b in boards)
            {
                b.OutputData = fs_data;
            }
        }

        /// <summary>Gets all output data.</summary>
        /// <returns>A dictionary of all outputs from every registered board.</returns>
        public Dictionary<string, string> GetAllOutputData()
        {
            Dictionary<string, string> all_data = new();

            foreach (BoardController.BoardDetails b in boards)
            {
                if (b.OutputData is not null)
                {
                    all_data = all_data.MergeLeft(b.OutputData);
                }
            }

            return all_data;
        }

        /// <summary>Gets the boards.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public List<BoardDetails> GetBoards()
        {
            Console.WriteLine("Im gettign the boards");
            return boards;
        }
    }
}
