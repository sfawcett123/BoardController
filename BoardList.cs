using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BoardController
{
    public abstract class BoardList 
    {
        private readonly List<BoardDetails> _boards = new() ;
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
                Rate = 1
            };

            if (!_boards.Contains<BoardDetails>(_bd))
            {
                _boards.Add(_bd);
            }

            return _bd.ToDictionary().Serialize();

        }
        public void RemoveTimedOut()
        {
            foreach (BoardController.BoardDetails b in _boards.Where(s => s.Timedout() == true).ToArray<BoardDetails>())
            {
                _boards.Remove(b);
                b.Dispose();
            }
        }
        public string Serialize()
        {
            return JsonSerializer.Serialize(this._boards);
        }
        public void OutputData(Dictionary<string, string> fs_data)
        {
            foreach (BoardController.BoardDetails b in _boards )
            {
                b.OutputData = fs_data;
            }
        }
    }
}
