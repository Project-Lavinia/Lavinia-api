using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LaviniaApi.Utilities;

// API v2
namespace LaviniaApi.Models
{
    public class ListElement<T>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Key { get; set; }
        public T Value { get; set; }
    }
}