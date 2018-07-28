using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaviniaApi.Models
{
    public class Party
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PartyId { get; set; }

        [Required] public string Name { get; set; }

        public string InternationalName { get; set; }
        public string ShortName { get; set; }
    }
}