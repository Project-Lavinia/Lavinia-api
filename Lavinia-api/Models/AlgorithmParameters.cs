using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LaviniaApi.Utilities;

// API v2
namespace LaviniaApi.Models
{
    // Contains the parameters used for distributing seats during an election.
    public class AlgorithmParameters
    {
        // ID Only for use by the Database
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // A string identifier that uniquely describes which algorithm to use.
        // See AlgorithmUtilities.cs for more information
        public string Algorithm { get; set; }

        // A list containing tuples of parameter names and value for the different parameters of an algorithm.
        // For algorithms not using any parameters the list should be empty
        public List<ListElement<double>> Parameters { get; set; }
    }
}