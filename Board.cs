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
        public string OperatingSystem { get => operatingSystem;  set => operatingSystem = ValidateOS(value); }
        public static string ValidateOS(string os)
        {
            if (os == null) { return OperatingSystems.UNKNOWN.ToString(); }

            foreach (string name in Enum.GetNames(typeof(OperatingSystems)))
            {
                if (os.ToUpper() == name) { return name; }
            }

            return OperatingSystems.UNKNOWN.ToString();
        }

        private string operatingSystem = "UNKNOWN";
    }

    public enum OperatingSystems
    {
        UNKNOWN,
        LINUX,
        ARDUINO,
        WINDOWS,
        RASPBERRY_PI
    }
}
