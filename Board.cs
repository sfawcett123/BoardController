using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardController
{
    /// <summary> Board Details </summary>
    public class Board
    {
        /// <summary>
        /// Name of the board
        /// </summary>
        [Required]
        [MaxLength(140)]
        public string Name { get; set; } = "unknown";
        
        /// <summary>
        /// List of Outputs
        /// </summary>
        /// <example>["PLANE ALTITUDE"]</example>
        public List<string>? Outputs { get; set; }

        /// <summary>
        /// The Operating System of the attached board.
        /// </summary>
        /// <example>ARDUINO</example>
        public string OperatingSystem { get => operatingSystem;  set => operatingSystem = ValidateOS(value); }
        
        /// <summary>
        /// Validate Operating System
        /// </summary>
        /// <param name="os"></param>
        /// <returns>Name of Operating system or UNKNOWN</returns>
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

    /// <summary>
    /// Operating systems supported by API
    /// </summary>
    public enum OperatingSystems
    {
        /// <summary>
        /// Other Unidentifed systems
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// Linux Based Systems
        /// </summary>
        LINUX,
        /// <summary>
        /// Arduino Based Systems
        /// </summary>
        ARDUINO,
        /// <summary>
        /// Windows Based Systems
        /// </summary>
        WINDOWS,
        /// <summary>
        /// Raspberry PI based systems
        /// </summary>
        RASPBERRY_PI
    }
}
