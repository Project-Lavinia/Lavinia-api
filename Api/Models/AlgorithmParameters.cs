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
        public int? Id { get; private set; }

        /// <summary>
        /// A string identifier that uniquely describes which algorithm to use.
        /// See AlgorithmUtilities.cs for more information
        /// </summary>
        public string Algorithm { get; private set; }

        /// <summary>
        /// A list containing tuples of parameter names and value for the different parameters of an algorithm.
        /// For algorithms not using any parameters the list should be empty
        /// </summary>
        public List<ListElement<double>> Parameters { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected AlgorithmParameters() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public AlgorithmParameters(
            string algorithm,
            List<ListElement<double>> parameters) : this()
        {
            Algorithm = algorithm;
            Parameters = parameters;
        }
    }
}