using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LaviniaApi.Models;

namespace LaviniaApi.Utilities
{
    public class CsvFileFormatException : FormatException
    {
        public CsvFileFormatException(string exText, string path, string line) : base(
            exText + "\nPath: " + path + "\nLine: " + line)
        {
        }
    }

    public static class CsvUtilities
    {
        public static List<T> CsvToList<T>(string filePath) where T : ICsvFormat<T>, new()
        {
            List<T> objects = new List<T>();
            StreamReader file = new StreamReader(filePath);
            FieldParser parser = new FieldParser(filePath, ";");
            file.ReadLine(); // Skip header string
            string currentLine;
            while ((currentLine = file.ReadLine()) != null)
            {
                if (!currentLine.Contains('#'))
                {
                    objects.Add(new T().Parse(currentLine, parser));
                }
            }
            file.Dispose();
            return objects;
        }

        // API v1
        /// <summary>
        ///     Reads any .csv file in the default format specified at https://www.valgresultat.no and creates a list of simple
        ///     string objects based on the schema.
        /// </summary>
        /// <param name="filePath">
        ///     The relative or absolute path of the file being read, has to be .csv and cannot contain
        ///     semicolon (;) in any of its fields as that is the delimiter used.
        /// </param>
        /// <returns>A list of VDModel objects that can be used for simple in-memory queries or populating a database.</returns>
        public static List<VDModel> CsvToList(string filePath)
        {
            List<VDModel> objects = new List<VDModel>();
            StreamReader file = new StreamReader(filePath);
            file.ReadLine(); // Skip header string
            string currentLine;
            while ((currentLine = file.ReadLine()) != null)
            {
                string[] objectFields = currentLine.Split(";");
                if (objectFields.Length != 18)
                {
                    throw new CsvFileFormatException(
                        $"Found a line with length {objectFields.Length} instead of the required 18.", filePath,
                        currentLine);
                }

                VDModel currentObject = new VDModel
                {
                    Fylkenummer = objectFields[0],
                    Fylkenavn = objectFields[1],
                    Kommunenummer = objectFields[2],
                    Kommunenavn = objectFields[3],
                    Stemmekretsnummer = objectFields[4],
                    Stemmekretsnavn = objectFields[5],
                    Partikode = objectFields[6],
                    Partinavn = objectFields[7],
                    OppslutningProsentvis = objectFields[8],
                    AntallStemmeberettigede = objectFields[9],
                    AntallForhåndsstemmer = objectFields[10],
                    AntallValgtingstemmer = objectFields[11],
                    AntallStemmerTotalt = objectFields[12],
                    EndringProsentSisteTilsvarendeValg = objectFields[13],
                    EndringProsentSisteEkvivalenteValg = objectFields[14],
                    AntallMandater = objectFields[15],
                    AntallUtjevningsmandater = objectFields[16]
                };
                objects.Add(currentObject);
            }
            file.Dispose();
            return objects;
        }

        // API v1
        /// <summary>
        ///     Reads any .csv file in the default format specified at https://www.valgresultat.no and creates an array of VDModel
        ///     objects based on the schema.
        /// </summary>
        /// <param name="filePath">
        ///     The relative or absolute path of the file being read, has to be .csv and cannot contain
        ///     semicolon (;) in any of its fields as that is the delimiter used.
        /// </param>
        /// <returns>An array of VDModel objects that can be used for simple in-memory queries or populating a database.</returns>
        public static VDModel[] CsvToVdArray(string filePath)
        {
            List<VDModel> objects = new List<VDModel>();
            StreamReader file = new StreamReader(filePath);
            file.ReadLine(); // Skip header string
            string currentLine;
            while ((currentLine = file.ReadLine()) != null)
            {
                string[] objectFields = currentLine.Split(";");
                if (objectFields.Length != 18)
                {
                    throw new CsvFileFormatException(
                        $"Found a line with length {objectFields.Length} instead of the required 18.", filePath,
                        currentLine);
                }

                VDModel currentObject = new VDModel
                {
                    Fylkenummer = objectFields[0],
                    Fylkenavn = objectFields[1],
                    Kommunenummer = objectFields[2],
                    Kommunenavn = objectFields[3],
                    Stemmekretsnummer = objectFields[4],
                    Stemmekretsnavn = objectFields[5],
                    Partikode = objectFields[6],
                    Partinavn = objectFields[7],
                    OppslutningProsentvis = objectFields[8],
                    AntallStemmeberettigede = objectFields[9],
                    AntallForhåndsstemmer = objectFields[10],
                    AntallValgtingstemmer = objectFields[11],
                    AntallStemmerTotalt = objectFields[12],
                    EndringProsentSisteTilsvarendeValg = objectFields[13],
                    EndringProsentSisteEkvivalenteValg = objectFields[14],
                    AntallMandater = objectFields[15],
                    AntallUtjevningsmandater = objectFields[16]
                };
                objects.Add(currentObject);
            }
            file.Dispose();
            return objects.ToArray<VDModel>();
        }
    }
}