using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOTMUmbracoBackend.Models
{
    public class Club
    {
        public int clubId { get; set; }
        public string clubName { get; set; }
        public string clubAddress { get; set; }
        public string clubCity { get; set; }
        public string clubLogo { get; set; }
        public string clubImage { get; set; }
        public List<Sponsor> Sponsors { get; set; }
        public List<Team> Teams { get; set; }
        //public List<Match> Matches { get; set; }
    }
}