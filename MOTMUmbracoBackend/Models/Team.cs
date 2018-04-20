using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MOTMUmbracoBackend.Models;

namespace MOTMUmbracoBackend.Models
{
    public class Team
    {
        public int teamId { get; set; }
        public string teamName { get; set; }
        public string teamSport { get; set; }
        public List<Match> teamMatches { get; set; }
    }
}