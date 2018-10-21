namespace LaviniaApi.Models
{
    // API v1
    public class VDModel
    {
        public long Id { get; set; }
        public string Fylkenummer { get; set; }
        public string Fylkenavn { get; set; }
        public string Kommunenummer { get; set; }
        public string Kommunenavn { get; set; }
        public string Stemmekretsnummer { get; set; }
        public string Stemmekretsnavn { get; set; }
        public string Partikode { get; set; }
        public string Partinavn { get; set; }
        public string OppslutningProsentvis { get; set; }
        public string AntallStemmeberettigede { get; set; }
        public string AntallForhåndsstemmer { get; set; }
        public string AntallValgtingstemmer { get; set; }
        public string AntallStemmerTotalt { get; set; }
        public string EndringProsentSisteTilsvarendeValg { get; set; }
        public string EndringProsentSisteEkvivalenteValg { get; set; }
        public string AntallMandater { get; set; }
        public string AntallUtjevningsmandater { get; set; }
    }
}