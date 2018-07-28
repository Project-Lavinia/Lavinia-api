using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaviniaApi.Models
{
    public class County
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountyId { get; set; }

        [Required] public string Name { get; set; }

        public int Seats { get; set; }

        [Required] public List<Result> Results { get; set; }
    }
}