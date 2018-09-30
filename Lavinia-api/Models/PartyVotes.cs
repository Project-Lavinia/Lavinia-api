using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaviniaApi.Models
{
    public class PartyVotes
    {
        public string Party { get; set; }
        public int Votes { get; set; }
        public string District { get; set; }
        public int ElectionYear { get; set; }
        public string ElectionType { get; set; }
    }
}