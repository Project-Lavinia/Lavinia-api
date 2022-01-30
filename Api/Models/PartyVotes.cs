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
        public string Party { get; init; } = default!;

        /// <summary>
        /// Number of votes the party received in the election
        /// </summary>
        public int Votes { get; init; } = default!;

        /// <summary>
        /// District in which the party received the votes
        /// </summary>
        public string District { get; init; } = default!;

        /// <summary>
        /// Election year in which the party received the votes
        /// </summary>
        public int ElectionYear { get; init; } = default!;

        /// <summary>
        /// Which type of election the votes were cast in
        /// </summary>
        public string ElectionType { get; init; } = default!;
    }
}