using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOTMUmbracoBackend.Models
{
    public class Vote
    {
        public int voteId { get; set; }
        public string deviceId { get; set; }
        public int playerId { get; set; }
    }
}