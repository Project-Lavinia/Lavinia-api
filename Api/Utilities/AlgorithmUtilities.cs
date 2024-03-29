﻿using System;
using System.Linq;

namespace Lavinia.Api.Utilities
{
    /// <summary>
    ///     Utility class to make operations surrounding the Algorithm enum more practical
    /// </summary>
    public static class AlgorithmUtilities
    {
        /// <summary>
        /// Our internal textual representations of the algorithm names
        /// </summary>
        public const string ModifiedSainteLagues = "Sainte Laguës (modified)";

        public const string SainteLagues = "Sainte Laguës";
        public const string DHondt = "d'Hondt";
        public const string Undefined = "Undefined Algorithm";

        /// <summary>
        /// Our accepted names for the different algorithms
        /// </summary>
        private static readonly string[] ModifiedSainteLaguesSet = { ModifiedSainteLagues.ToLowerInvariant() };

        private static readonly string[] SainteLaguesSet = { SainteLagues.ToLowerInvariant() };
        private static readonly string[] DHondtSet = { DHondt.ToLowerInvariant() };

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
            else if (DHondtSet.Contains(curName))
            {
                return DHondt;
            }
            else
            {
                throw new ArgumentException($"{name} is not a valid algorithm name.");
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

        /// <summary>
        ///     Attempts to convert the name to an algorithm.
        ///     If it is successful it returns true and pushes the Algorithm to the algorithm variable.
        ///     Otherwise it returns false and pushes Algorithm.Undefined to the algorithm variable.
        /// </summary>
        /// <param name="name">The name of the algorithm.</param>
        /// <param name="algorithm">Where the algorithm should be returned.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool TryParseToString(string name, out string? algorithm)
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