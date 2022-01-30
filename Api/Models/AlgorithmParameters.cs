using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lavinia.Api.Models
{
    /// <summary>
    /// Contains the parameters used for distributing seats during an election.
    /// </summary>
    public class AlgorithmParameters
    {
        /// <summary>
        /// ID Only for use by the Database
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// A string identifier that uniquely describes which algorithm to use.
        /// See AlgorithmUtilities.cs for more information
        /// </summary>
        public string Algorithm { get; set; }

        /// <summary>
        /// A list containing tuples of parameter names and value for the different parameters of an algorithm.
        /// For algorithms not using any parameters the list should be empty
        /// </summary>
        public List<ListElement<double>> Parameters { get; set; }
    }
}