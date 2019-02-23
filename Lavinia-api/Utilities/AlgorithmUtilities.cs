using System;
using System.Linq;
using System.Security.Policy;

namespace LaviniaApi.Utilities
{
    /// API V1
    /// <summary>
    ///     Enum representing the available methods of seat calculation for political systems given a voting district/circuit.
    /// </summary>
    public enum Algorithm
    {
        /// <summary>
        ///     Algorithm not defined
        /// </summary>
        Undefined = 0,

        /// <summary>
        ///     The Modified Sainte-Lagüe method in accordance with the Norwegian system. TODO: More accurate description of
        ///     Modified Sainte-Lagüe
        /// </summary>
        ModifiedSainteLagues = 1,

        /// <summary>
        ///     Normal Sainte-Lagüe method TODO: More accurate description of Sainte-Lagüe
        /// </summary>
        SainteLagues = 3,

        /// <summary>
        ///     D'Hondt method TODO: More accurate description of D'Hondt
        /// </summary>
        DHondt = 2
    }

    /// <summary>
    ///     Utility class to make operations surrounding the Algorithm enum more practical
    /// </summary>
    public static class AlgorithmUtilities
    {
        // Our internal textual representations of the algorithm names
        public const string ModifiedSainteLagues = "Sainte Laguës (modified)";
        public const string SainteLagues = "Sainte Laguës";
        public const string DHondt = "d'Hondt";
        public const string Undefined = "Undefined Algorithm";

        // Our accepted names for the different algorithms
        private static readonly string[] ModifiedSainteLaguesSet = {ModifiedSainteLagues.ToLowerInvariant()};
        private static readonly string[] SainteLaguesSet = {SainteLagues.ToLowerInvariant()};
        private static readonly string[] DHondtSet = {DHondt.ToLowerInvariant()};

        /// API V1
        /// <summary>
        ///     Accepts a string and returns the matching algorithm enum.
        ///     If no matching enum can be found it throws an ArgumentException.
        /// </summary>
        /// <param name="name">The name of the algorithm to be converted.</param>
        /// <returns>An algorithm enum</returns>
        private static Algorithm StringToAlgorithm(string name)
        {
            string curName = name.ToLowerInvariant();

            if (ModifiedSainteLaguesSet.Contains(curName))
            {
                return Algorithm.ModifiedSainteLagues;
            }

            if (SainteLaguesSet.Contains(curName))
            {
                return Algorithm.SainteLagues;
            }

            if (DHondtSet.Contains(curName))
            {
                return Algorithm.DHondt;
            }

            throw new ArgumentException($"{name} is not a valid algorithm name.");
        }

        /// API V2
        /// <summary>
        ///     Accepts a string and returns the matching algorithm enum.
        ///     If no matching enum can be found it throws an ArgumentException.
        /// </summary>
        /// <param name="name">The name of the algorithm to be converted.</param>
        /// <returns>An algorithm enum</returns>
        private static string NormaliseAlgorithm(string name)
        {
            string curName = name.ToLowerInvariant();

            if (ModifiedSainteLaguesSet.Contains(curName))
            {
                return ModifiedSainteLagues;
            }

            if (SainteLaguesSet.Contains(curName))
            {
                return SainteLagues;
            }

            if (DHondtSet.Contains(curName))
            {
                return DHondt;
            }

            throw new ArgumentException($"{name} is not a valid algorithm name.");
        }


        /// API V1
        /// <summary>
        ///     Accepts an algorithm enum and returns our internal textual representation of that algorithm.
        ///     If there is not any known textual representation of it an ArgumentException is thrown.
        /// </summary>
        /// <param name="algorithm">The Algorithm enum to be converted.</param>
        /// <returns>The name of the algorithm enum</returns>
        public static string AlgorithmToString(Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Algorithm.ModifiedSainteLagues:
                    return ModifiedSainteLagues;
                case Algorithm.SainteLagues:
                    return SainteLagues;
                case Algorithm.DHondt:
                    return DHondt;
                default:
                    throw new ArgumentException($"{algorithm} does not have a string name.");
            }
        }

        /// <summary>
        ///     Checks whether the name matches any algorithm we know.
        /// </summary>
        /// <param name="name">The name of the algorithm.</param>
        /// <returns>True if we have an enum for the algorithm, false otherwise.</returns>
        private static bool IsAlgorithm(string name)
        {
            string curName = name.ToLowerInvariant();
            return ModifiedSainteLaguesSet.Contains(curName)
                   || SainteLaguesSet.Contains(curName)
                   || DHondtSet.Contains(curName);
        }

        /// API V1
        /// <summary>
        ///     Attempts to convert the name to an algorithm.
        ///     If it is successful it returns true and pushes the Algorithm to the algorithm variable.
        ///     Otherwise it returns false and pushes Algorithm.Undefined to the algorithm variable.
        /// </summary>
        /// <param name="name">The name of the algorithm.</param>
        /// <param name="algorithm">Where the algorithm should be returned.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool TryParse(string name, out Algorithm algorithm)
        {
            if (IsAlgorithm(name))
            {
                algorithm = StringToAlgorithm(name);
                return true;
            }

            algorithm = Algorithm.Undefined;
            return false;
        }

        /// API V2
        /// <summary>
        ///     Attempts to convert the name to an algorithm.
        ///     If it is successful it returns true and pushes the Algorithm to the algorithm variable.
        ///     Otherwise it returns false and pushes Algorithm.Undefined to the algorithm variable.
        /// </summary>
        /// <param name="name">The name of the algorithm.</param>
        /// <param name="algorithm">Where the algorithm should be returned.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool TryParseToString(string name, out string algorithm)
        {
            if (IsAlgorithm(name))
            {
                algorithm = NormaliseAlgorithm(name);
                return true;
            }

            algorithm = Undefined;
            return false;
        }
    }
}