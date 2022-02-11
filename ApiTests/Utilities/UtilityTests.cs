using System.IO;
using Xunit;

namespace Lavinia.Api.Utilities.Tests
{
    public class UtilityTests
    {
        private const string FilePath = "2017.csv";

        private const string ExpectedHeaderString = "Fylkenummer;Fylkenavn;Kommunenummer;Kommunenavn;Stemmekretsnummer;Stemmekretsnavn;Partikode;Partinavn;Oppslutning prosentvis;Antall stemmeberettigede;Antall forhåndsstemmer;Antall valgtingstemmer;Antall stemmer totalt;Endring % siste tilsvarende valg;Endring % siste ekvivalente valg;Antall mandater;Antall utjevningsmandater;";

        [Fact]
        public void ReadHeadersTest()
        {
            StreamReader file = new(FilePath);
            string? actualHeaderString = file.ReadLine();
            file.Dispose();
            Assert.Equal(ExpectedHeaderString, actualHeaderString);
        }
    }
}