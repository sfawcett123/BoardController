using Microsoft.AspNetCore.Http;
using System.Text.Json;


namespace BoardController
{
 /// <summary>
 /// List of Boards
 /// </summary>
    public abstract class BoardList 
    {
        /// <summary>The boards</summary>
        public readonly List<BoardDetails> boards = new() ;

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
                if (_request.RemoteIpAddress is not null)
                    _ip_address = _request.RemoteIpAddress.ToString();

            Console.WriteLine("Request from " + _ip_address + " Name = " + _board.Name);

            BoardDetails _bd = new()
            {
                Name = _board.Name,
                IPAddress = _ip_address,
                Rate = 1,
                OS = _board.OperatingSystem,
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
                boards.Remove(b);
                b.Dispose();
            }
        }
        /// <summary>
        /// Convert board data to JSON
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return JsonSerializer.Serialize(this.boards);
        }
        /// <summary>
        /// For each board set the output data
        /// </summary>
        /// <param name="fs_data"></param>
        
        public void OutputData(Dictionary<string, string> fs_data)
        {
            foreach (BoardController.BoardDetails b in boards )
            {
                b.OutputData = fs_data;
            }
        }
    }
}
