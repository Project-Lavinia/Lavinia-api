using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaviniaApi.Models
{
    // API v1
    public class ElectionType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ElectionTypeId { get; set; }

        [Required] public string InternationalName { get; set; }

        [Required] public string ElectionTypeCode { get; set; }

        [Required] public virtual List<Election> Elections { get; set; }
    }
}