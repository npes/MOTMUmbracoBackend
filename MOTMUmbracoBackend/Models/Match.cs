using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOTMUmbracoBackend.Models
{
    public class Match
    {
        public int matchId { get; set; }
        public string matchAddress { get; set; }

        public string matchHomeTeam { get; set; }
        public string matchCity { get; set; }
        public string matchStartDateTime { get; set; }
        public string opponent { get; set; }
        public string status { get; set; }
        public int homeGoal { get; set; }
        public int opponentGoal { get; set; }
    }
}