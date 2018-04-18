using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Umbraco.Web.WebApi;
using Umbraco.Core.Models;
//using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text;

namespace MOTMUmbracoBackend.Controllers
{
    public class TestApiController : UmbracoApiController
    {

        // GET: Club by club id OK
        [HttpGet] 
        public Club GetClub(int cID)
        {
            var cs = Services.ContentService;
            var club = cs.GetById(cID);
            if (club != null)
            {
                var c = new Club();
                c.ClubId = club.Id;
                c.clubName = (club.Properties["clubName"].Value != null) ? club.Properties["clubName"].Value.ToString() : "No club name found";
                c.clubAddress = (club.Properties["clubAddress"].Value != null) ? club.Properties["clubAddress"].Value.ToString() : "No club address found";
                c.clubCity = (club.Properties["clubCity"].Value != null) ? club.Properties["clubCity"].Value.ToString() : "No club city found";
                try
                {
                    c.clubLogo = this.getImg(club.Properties["clubLogo"].Value.ToString());
                }
                catch
                {
                    c.clubLogo = "/media/1001/M1.png";
                };
                try
                {
                    c.clubImage = this.getImg(club.Properties["clubImage"].Value.ToString());
                }
                catch
                {
                    c.clubImage = "/media/1001/M1.png";
                };

                return c;
            }
            else
            {
                return new Club();
            }
        }

        //GET: Team by team id OK
        public Team GetTeam(int tID)
        {
            var cs = Services.ContentService;
            var team = cs.GetById(tID);
            if (team !=null)
            {
                var t = new Team();
                t.TeamId = team.Id;
                t.TeamName = (team.Properties["teamName"].Value != null) ? team.Properties["teamName"].Value.ToString() : "Team name";
                return t;
            }
            else
            {
                return new Team();
            }
        }

        // Get all teams from club by club id OK
        public List<Team> GetTeams(int cID)
        {
            var cs = Services.ContentService;
            List<Team> teamlist = new List<Team>();
            var teams = cs.GetById(cID).Descendants().Where(t => t.ContentType.Alias.Equals("team"));
            

                foreach (var team in teams)
                {
                    var t = new Team();
                    t.TeamId = team.Id;
                    t.TeamName = (team.Properties["teamName"].Value != null) ? team.Properties["teamName"].Value.ToString() : "Team name";
                    teamlist.Add(t);
                }
                return teamlist;            
        }

        // Get players by team id
        public List<Player> GetTeamPlayers(int cID)
        {
            var cs = Services.ContentService;
            List<Player> playerlist = new List<Player>();
            //var players = cs.GetById(tID).Children().Where(t => t.ContentType.Alias.Equals("player"));
            var players = cs.GetById(cID).Descendants().Where(t => t.ContentType.Alias.Equals("player"));

            foreach (var player in players)
            {
                var p = new Player();
                var t = new Team();
                t = GetPlayerTeam(player.Properties["team"].Value.ToString());
                p.PlayerId = player.Id;
                p.playerFirstName = (player.Properties["playerFirstName"].Value != null) ? player.Properties["playerFirstName"].Value.ToString() : "Player first name";
                p.playerLastName = (player.Properties["playerLastName"].Value != null) ? player.Properties["playerLastName"].Value.ToString() : "Player last name";
                p.playerNumber = (player.Properties["playerNumber"].Value != null) ? int.Parse(player.Properties["playerNumber"].Value.ToString()) : 0;
                p.teamId = t.TeamId;
                p.teamName = t.TeamName;
                playerlist.Add(p);
            }
            return playerlist;
        }

        [Umbraco.Web.WebApi.UmbracoAuthorize]
        public List<Club> GetAllClubs (int rootID)
        {
            var cs = Services.ContentService;
            List<Club> allClubs = new List<Club>();
            var clubs = cs.GetById(rootID).Children();
            foreach (var club in clubs)
            {
                var c = new Club();
                c.ClubId = club.Id;
                c.clubName = (club.Properties["clubName"].Value != null) ? club.Properties["clubName"].Value.ToString() : "No club name found";
                c.clubAddress = (club.Properties["clubAddress"].Value != null) ? club.Properties["clubAddress"].Value.ToString() : "No club address found";
                c.clubCity = (club.Properties["clubCity"].Value != null) ? club.Properties["clubCity"].Value.ToString() : "No club city found";
                try
                {
                    c.clubLogo = this.getImg(club.Properties["clubLogo"].Value.ToString());
                }
                catch
                {
                    c.clubLogo = "/media/1001/M1.png";
                };
                try
                {
                    c.clubImage = this.getImg(club.Properties["clubImage"].Value.ToString());
                }
                catch
                {
                    c.clubImage = "/media/1001/M1.png";
                };
                allClubs.Add(c);
            }
            return allClubs;
        }


        //POST METHODS

        //Post Club
        [HttpPost]
        public void PostClub(String club)
        {
            Club t = JsonConvert.DeserializeObject<Club>(club);
            Services.ContentService.CreateContent(t.clubName, 1083, "Club");
        }

        //Post team in club - OK
        [Umbraco.Web.WebApi.UmbracoAuthorize]
        [HttpPost]
        public Team PostTeam([FromBody] Team data, int cID)
        {
            var cs = Services.ContentService;

            var newTeam = cs.CreateContent(data.TeamName, cID, "Team");
            //NewTeam.SetValue("teamName", data.TeamName); //populate value
            cs.SaveAndPublishWithStatus(newTeam);
            Team createdTeam = new Team
            {
                TeamId = newTeam.Id,
                TeamName = newTeam.Name
            };
            //return Request.CreateResponse<string>(HttpStatusCode.OK, "Id: " + newTeam.Id.ToString() + " TeamName: " + newTeam.Name);
            //return Request.CreateResponse<String>(HttpStatusCode.OK, (JsonConvert.SerializeObject(createdTeam)));
            return createdTeam;
        }

        //Update team with team id - OK
        [Umbraco.Web.WebApi.UmbracoAuthorize]
        [HttpPut]
        public object UpdateTeam([FromBody] Team data, int tID)
        {
            if (tID != 0)
            {
                var cs = Services.ContentService;
                var currentTeam = cs.GetById(tID);
                currentTeam.SetValue("teamName", data.TeamName);
                currentTeam.Name = data.TeamName; //the umbraco document name!!
                cs.SaveAndPublishWithStatus(currentTeam);
                return currentTeam;
            }
            else
            {
                return new HttpError();
            }
        }

        //Move team to recyclebin with team id - OK
        [Umbraco.Web.WebApi.UmbracoAuthorize]
        [HttpDelete]
        public object DeleteTeam(int tID)
        {
            var cs = Services.ContentService;
            var currentTeam = cs.GetById(tID);
            cs.MoveToRecycleBin(currentTeam);
            //cs.SaveAndPublishWithStatus(currentTeam);

            //Team updatedTeam = new Team
            //{
            //    TeamId = currentTeam.Id,
            //    TeamName = currentTeam.Name
            //};

            //return Request.CreateResponse<string>(HttpStatusCode.OK, "Id: " + currentTeam.Id.ToString() + " TeamName: " + currentTeam.Name);
            return currentTeam;
        }

        //HELPER METHODS

        private string getImg(string guidString)
        {
            var ms = Services.MediaService;

            var imgGuid = Guid.Parse(guidString.Substring(12));

            var img = ms.GetById(imgGuid);

            return Umbraco.Media(img.Id).Url;
        }

        private Team GetPlayerTeam (string contentGuid)
        {
            var cs = Services.ContentService;
            var nodeGuid = Guid.Parse(contentGuid.Substring(15));
            var nodeId = cs.GetById(nodeGuid);
            var team = new Team
            {
                TeamId = Umbraco.Content(nodeId.Id).Id,
                TeamName = Umbraco.Content(nodeId.Id).Name
            };
            return team;
        }

        //CLASSES
        public class Club
        {
            public int ClubId { get; set; }
            public string clubName { get; set; }
            public string clubAddress { get; set; }
            public string clubCity { get; set; }
            public string clubLogo { get; set; }
            public string clubImage { get; set; }
            
        }

        public class Team
        {
            public int TeamId { get; set; }
            public string TeamName { get; set; }
        }

        public class Player
        {
            public int PlayerId { get; set; }
            public string playerFirstName { get; set; }
            public string playerLastName { get; set; }
            public int playerNumber { get; set; }
            public int teamId { get; set; }
            public string teamName { get; set; }
        }
    }
}