// ***********************************************************************
// Assembly         : BoardManager
// Author           : steve
// Created          : 12-30-2022
//
// Last Modified By : steve
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="Board.cs" company="BoardManager">
//     Steven Fawcett
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;

namespace BoardManager
{
    /// <summary>
    /// API provide new board details
    /// </summary>
    public sealed class Board
    {
        /// <summary>
        /// Name of the new board
        /// </summary>
        /// <value>The name.</value>
        [Required]
        [MaxLength(140)]
        public string Name { get; set; } = "unknown";

        /// <summary>
        /// List of Outputs
        /// </summary>
        /// <value>The outputs.</value>
        /// <example>["PLANE ALTITUDE"]</example>
        public List<string>? Outputs { get; set; }

        /// <summary>
        /// The Operating System of the attached board.
        /// </summary>
        /// <value>The operating system.</value>
        /// <example>ARDUINO</example>
        public string OperatingSystem { get => operatingSystem; set => operatingSystem = ValidateOS(value); }

        /// <summary>
        /// Validate Operating System
        /// </summary>
        /// <param name="os">The os.</param>
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

        /// <summary>
        /// The operating system
        /// </summary>
        private string operatingSystem = "UNKNOWN";
    }

    /// <summary>
    /// Operating systems supported by API
    /// </summary>
    public enum OperatingSystems
    {
        /// <summary>
        /// Other Unidentified systems
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
