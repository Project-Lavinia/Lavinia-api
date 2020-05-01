using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaviniaApi.Models
{
    // API v3
    // Stores all the parties in the API
    public class Party
    {
        // The party code
        [Key]
        public string Code { get; set; }

        // The full name of the party
        public string Name { get; set; }
    }
}