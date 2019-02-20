using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaviniaApi.Models
{
    // API v2
    // Stores how many votes a party received during an election on a per district basis.
    public class PartyVotes
    {
        // Party code of the relevant party
        public string Party { get; set; }

        // Number of votes the party received in the election
        public int Votes { get; set; }

        // District in which the party received the votes
        public string District { get; set; }

        // Election year in which the party received the votes
        public int ElectionYear { get; set; }

        // Which type of election the votes were cast in
        public string ElectionType { get; set; }
    }
}