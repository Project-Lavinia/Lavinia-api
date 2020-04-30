using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaviniaApi.Models
{
    // API v2
    public class Party
    {
        [Required] public string Name { get; set; }
        [Required] public string Code { get; set; }
    }
}