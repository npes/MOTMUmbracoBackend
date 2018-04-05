using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace MOTMUmbracoBackend.Controllers
{
    public class TestApiController : UmbracoApiController
    {
        // GET: Club
        [HttpGet] 
        public Club GetClub(int cID)
        {
            var cs = Services.ContentService;
            var club = cs.GetById(cID);
            if (club != null)
            {
                var c = new Club();
                c.Id = club.Id;
                c.ClubName = (club.Properties["clubName"].Value != null) ? club.Properties["clubName"].Value.ToString() : "Clubname";
                c.ClubAddress = (club.Properties["clubAddress"].Value != null) ? club.Properties["clubAddress"].Value.ToString() : "Address";
                try
                {
                    c.ClubLogo = this.getImg(club.Properties["clubLogo"].Value.ToString());
                }
                catch
                {
                    c.ClubLogo = "/media/1001/M1.png";
                };
                try
                {
                    c.ClubImage = this.getImg(club.Properties["clubImage"].Value.ToString());
                }
                catch
                {
                    c.ClubImage = "/media/1001/M1.png";
                };

                return c;
            }
            else
            {
                return new Club();
            }
        }

        //GET: Team
        public Team GetTeam(int tID)
        {
            var cs = Services.ContentService;
            var team = cs.GetById(tID);
            if (team !=null)
            {
                var t = new Team();
                t.Id = team.Id;
                t.TeamName = (team.Properties["teamName"].Value != null) ? team.Properties["teamName"].Value.ToString() : "Team name";
                return t;
            }
            else
            {
                return new Team();
            }
        }

        private string getImg(string guidString)
        {
            var ms = Services.MediaService;

            var imgGuid = Guid.Parse(guidString.Substring(12));

            var img = ms.GetById(imgGuid);

            return Umbraco.Media(img.Id).Url;
        }

        public class Club
        {
            public int Id { get; set; }
            public string ClubName { get; set; }
            public string ClubAddress { get; set; }
            public string ClubImage { get; set; }
            public string ClubLogo { get; set; }
        }

        public class Team
        {
            public int Id { get; set; }
            public string TeamName { get; set; }
        }
    }
}