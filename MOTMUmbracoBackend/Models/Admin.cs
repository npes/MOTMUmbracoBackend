using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOTMUmbracoBackend.Models
{
    public class Admin
    {
        public string UserGroup { get; set; }
        public string Username { get; set; }
        public string UserFriendlyName { get; set; }
        public string Password { get; set; }
        public int ClubId { get; set; }

    }
}