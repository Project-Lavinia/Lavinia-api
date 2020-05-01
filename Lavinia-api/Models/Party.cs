using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaviniaApi.Models
{
    // API v3
    // Stores all the parties in the API
    public class Party
    {
        // The full name of the party
        [Key]
        public string Name { get; set; }

        // The party code
        public string Code { get; set; }
    }
}