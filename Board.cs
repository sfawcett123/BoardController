using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardController
{
    public class Board
    {
        [Required]
        [MaxLength(140)]
        public string Name { get; set; } = "unknown";
        public List<string>? Outputs { get; set; }
        public string Operating_system { get; set; } = "unknown";
    }
}
