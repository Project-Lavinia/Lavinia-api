using System.Collections.Generic;
using System.IO;
using LaviniaApi.Models;
using LaviniaApi.Utilities;
using Xunit;

namespace LaviniaApi.Tests
{
    public class UtilityTests
    {
        private const string FilePath = "../../../Data/Countries/NO/PE/2017.csv";

        private const string ExpectedHeaderString = "Fylkenummer;Fylkenavn;Kommunenummer;Kommunenavn;Stemmekretsnummer;Stemmekretsnavn;Partikode;Partinavn;Oppslutning prosentvis;Antall stemmeberettigede;Antall forhåndsstemmer;Antall valgtingstemmer;Antall stemmer totalt;Endring % siste tilsvarende valg;Endring % siste ekvivalente valg;Antall mandater;Antall utjevningsmandater;";

        // API v1
        [Fact]
        public void ReadAllTest()
        {
            int expectedNumberOfVotes = 800947; // from VDs own site
            List<VDModel> objects = CsvUtilities.CsvToList(FilePath);
            List<VDModel> APResults = objects.FindAll(a => a.Partinavn == "Arbeiderpartiet");
            int actualNumberOfVotes = 0;
            foreach (VDModel element in APResults)
            {
                int.TryParse(element.AntallStemmerTotalt, out int stemmer);
                actualNumberOfVotes += stemmer;
            }

            Assert.Equal(expectedNumberOfVotes, actualNumberOfVotes);
        }

        [Fact]
        public void ReadFileTest()
        {
            StreamReader streamReader = new StreamReader(FilePath);
            Assert.NotNull(streamReader);
        }

        // API v1
        [Fact]
        public void ReadFirstObjectTest()
        {
            StreamReader file = new StreamReader(FilePath);
            file.ReadLine(); // Skip the header string
            string objectLine = file.ReadLine();
            file.Dispose();
            string[] objectFields = objectLine.Split(";");
            VDModel vdModel = new VDModel
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
            Assert.Equal("Østfold", vdModel.Fylkenavn);
            Assert.Equal("3", vdModel.AntallMandater);
        }

        [Fact]
        public void ReadHeadersTest()
        {
            StreamReader file = new StreamReader(FilePath);
            string actualHeaderString = file.ReadLine();
            file.Dispose();
            Assert.Equal(ExpectedHeaderString, actualHeaderString);
        }
    }
}