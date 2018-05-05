using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOTMUmbracoBackend.Models
{
    public class Player
    {
        public int playerId { get; set; }
        public string playerFirstName { get; set; }
        public string playerLastName { get; set; }
        public int playerNumber { get; set; }
        public int teamId { get; set; }
        public string teamName { get; set; }
        public string roleType { get; set; }
    }
}