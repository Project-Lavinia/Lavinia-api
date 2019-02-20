using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LaviniaApi.Utilities;

// API v2
namespace LaviniaApi.Models
{
    // As EF does not support lists of native types, this class can be used to store values in a list
    // The class can hold just values, keys or a combination of both depending on need.
    // It can after retrieval from storage be converted to a Dictionary or List.
    public class ListElement<T>
    {
        // ID only for use by the Database
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // For storing an identifier for the value
        public string Key { get; set; }

        // For storing the actual value
        public T Value { get; set; }
    }
}