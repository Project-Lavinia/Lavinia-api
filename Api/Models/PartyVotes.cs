namespace Lavinia.Api.Models
{
    /// <summary>
    /// Stores how many votes a party received during an election on a per district basis.
    /// </summary>
    public class PartyVotes
    {
        /// <summary>
        /// Party code of the relevant party
        /// </summary>
        public string Party { get; set; }

        /// <summary>
        /// Number of votes the party received in the election
        /// </summary>
        public int Votes { get; set; }

        /// <summary>
        /// District in which the party received the votes
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// Election year in which the party received the votes
        /// </summary>
        public int ElectionYear { get; set; }

        /// <summary>
        /// Which type of election the votes were cast in
        /// </summary>
        public string ElectionType { get; set; }
    }
}