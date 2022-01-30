using System;
using System.Linq;

namespace Lavinia.Api.Utilities
{
    public static class ETNameUtilities
    {
        /// <summary>
        /// Our internal textual ElectionType codes
        /// </summary>
        private const string PARLIAMENTARY_ELECTION_CODE = "pe";

        /// <summary>
        /// Our printable ElectionType names
        /// </summary>
        private const string PARLIAMENTARY_ELECTION_NAME = "Parliamentary Election";

        /// <summary>
        /// Our accepted names for the different algorithms
        /// </summary>
        private static readonly string[] ParliamentaryElectionSet =
            {PARLIAMENTARY_ELECTION_CODE.ToLowerInvariant(), PARLIAMENTARY_ELECTION_NAME.ToLowerInvariant(), "stortingsvalg"};

        /// <summary>
        ///     Accepts a string and returns the matching code.
        ///     If no matching enum can be found it throws an ArgumentException.
        /// </summary>
        /// <param name="name">The name of the ElectionType code.</param>
        /// <returns>A string code</returns>
        private static string NameToCode(string name)
        {
            string lowerName = name.ToLowerInvariant();

            if (ParliamentaryElectionSet.Contains(lowerName))
            {
                return PARLIAMENTARY_ELECTION_CODE;
            }

            throw new ArgumentException($"{name} is not a valid ElectionType name.");
        }

        /// <summary>
        ///     Accepts an ElectionType code and returns the full name.
        ///     If the code is not recognized an ArgumentException is thrown.
        /// </summary>
        /// <param name="code">The ElectionType code to be converted.</param>
        /// <returns>The full name of the ElectionType</returns>
        public static string CodeToName(string code)
        {
            if (code == PARLIAMENTARY_ELECTION_CODE)
            {
                return PARLIAMENTARY_ELECTION_NAME;
            }

            throw new ArgumentException($"{code} is not recognized.");
        }

        /// <summary>
        ///     Checks whether the name matches any ElectionType we know.
        /// </summary>
        /// <param name="name">The name of the ElectionType.</param>
        /// <returns>True if the name is in our dictionary, false otherwise.</returns>
        private static bool IsElectionType(string name)
        {
            return ParliamentaryElectionSet.Contains(name);
        }

        /// <summary>
        ///     Attempts to convert the name to a code.
        ///     If it is successful it returns true and pushes the code to the code variable.
        ///     Otherwise it returns false and pushes null to the code variable.
        /// </summary>
        /// <param name="name">The name of the ElectionType.</param>
        /// <param name="code">Where the code should be returned.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool TryParse(string name, out string code)
        {
            if (IsElectionType(name))
            {
                code = NameToCode(name);
                return true;
            }

            code = null;
            return false;
        }
    }
}