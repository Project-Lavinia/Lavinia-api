using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LaviniaApi.Utilities;

// API v2
namespace LaviniaApi.Models
{
    public class AlgorithmParameters
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Algorithm Algorithm { get; set; }
        public double FirstDivisor { get; set; }
    }
}