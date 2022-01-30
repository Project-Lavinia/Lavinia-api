using System.ComponentModel.DataAnnotations;

namespace Lavinia.Api.Models
{
    /// <summary>
    /// Stores all the parties in the API
    /// </summary>
    public class Party
    {
        /// <summary>
        /// The party code
        /// </summary>
        [Key]
        public string Code { get; set; }

        /// <summary>
        /// The full name of the party
        /// </summary>
        public string Name { get; set; }
    }
}