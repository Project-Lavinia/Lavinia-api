using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaviniaApi.Models
{
    // API v1
    public class Party
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PartyId { get; set; }

        [Required] public string Name { get; set; }
        [Required] public string Code { get; set; }
    }
}