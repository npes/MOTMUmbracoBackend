using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOTMUmbracoBackend.Models
{
    public class Sponsor
    {
        public int sponsorId { get; set; }
        public string sponsorName { get; set; }
        public string sponsorAddress { get; set; }
        public string sponsorLogo { get; set; }
    }
}