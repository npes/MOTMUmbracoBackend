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
using MOTMUmbracoBackend.Models;

namespace MOTMUmbracoBackend.Controllers
{
    public class TestApiController : UmbracoApiController
    {
        public string apiUrl = "https://motmapi.nikolajsimonsen.com";
        // GET: Club by club id OK
        [HttpGet]
        public Club GetClub(int cID)
        {
            var cs = Services.ContentService;
            var club = cs.GetById(cID);
            List<Sponsor> sponsorlist = new List<Sponsor>();
            List<Team> teamlist = new List<Team>();
            
            if (club != null)
            {
                var c = new Club();
                c.clubId = club.Id;
                c.clubName = (club.Properties["clubName"].Value != null) ? club.Properties["clubName"].Value.ToString() : "No club name found";
                c.clubAddress = (club.Properties["clubAddress"].Value != null) ? club.Properties["clubAddress"].Value.ToString() : "No club address found";
                c.clubCity = (club.Properties["clubCity"].Value != null) ? club.Properties["clubCity"].Value.ToString() : "No club city found";
                var sponsors = cs.GetById(cID).Descendants().Where(t => t.ContentType.Alias.Equals("sponsor"));
                var teams = cs.GetById(cID).Descendants().Where(t => t.ContentType.Alias.Equals("team"));
                //var matches = cs.GetById(cID).Descendants().Where(t => t.ContentType.Alias.Equals("Match"));
                try
                {
                    c.clubLogo = apiUrl + this.getImg(club.Properties["clubLogo"].Value.ToString());
                }
                catch
                {
                    c.clubLogo = apiUrl + "/media/1001/M1.png";
                };
                

                foreach (var sponsor in sponsors)
                {
                    var s = new Sponsor();
                    s.sponsorId = sponsor.Id;
                    s.sponsorName = (sponsor.Properties["sponsorName"].Value != null) ? sponsor.Properties["sponsorName"].Value.ToString() : "No sponsor name found";
                    s.sponsorAddress = (sponsor.Properties["sponsorAddress"].Value != null) ? sponsor.Properties["sponsorAddress"].Value.ToString() : "No sponsor address found";
                    try
                    {
                        s.sponsorLogo = apiUrl + this.getImg(club.Properties["sponsorLogo"].Value.ToString());
                    }
                    catch
                    {
                        s.sponsorLogo = apiUrl + "/media/1001/M1.png";
                    };

                    sponsorlist.Add(s);
                }
                c.Sponsors = sponsorlist;

                foreach (var team in teams)
                {
                    var t = new Team();
                    t.teamId = team.Id;
                    t.teamName = (team.Properties["teamName"].Value != null) ? team.Properties["teamName"].Value.ToString() : "Team name";
                    List<Match> matchlist = new List<Match>();
                    var matches = cs.GetById(t.teamId).Descendants().Where(te => te.ContentType.Alias.Equals("match"));
                    foreach (var match in matches)
                    {
                        var m = new Match();
                        m.matchId = match.Id;
                        m.matchAddress = (match.Properties["matchAddress"].Value != null) ? match.Properties["matchAddress"].Value.ToString() : "Match Address";
                        m.matchCity = (match.Properties["matchCity"].Value != null) ? match.Properties["matchCity"].Value.ToString() : "Match City";
                        m.matchStartDateTime = (match.Properties["matchStartDateTime"].Value != null) ? match.Properties["matchStartDateTime"].Value.ToString() : "Match Start DateTime";
                        m.opponent = (match.Properties["opponent"].Value != null) ? match.Properties["opponent"].Value.ToString() : "Opponent";
                        m.homeGoal = int.Parse((match.Properties["homeGoal"].Value != null) ? match.Properties["homeGoal"].Value.ToString() : "0");
                        m.opponentGoal = int.Parse((match.Properties["opponentGoal"].Value != null) ? match.Properties["opponentGoal"].Value.ToString() : "0");

                        var matchStatus = (match.Properties["status"].Value != null) ? match.Properties["status"].Value.ToString() : "Status";
                        m.status = Umbraco.GetPreValueAsString(int.Parse(matchStatus));

                        matchlist.Add(m);
                    }
                    t.teamMatches = matchlist;
                    teamlist.Add(t);

                }
                c.Teams = teamlist;
                //c.Matches = matchlist;

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
                t.teamId = team.Id;
                t.teamName = (team.Properties["teamName"].Value != null) ? team.Properties["teamName"].Value.ToString() : "Team name";
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
                    t.teamId = team.Id;
                    t.teamName = (team.Properties["teamName"].Value != null) ? team.Properties["teamName"].Value.ToString() : "Team name";
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
                p.playerId = player.Id;
                p.playerFirstName = (player.Properties["playerFirstName"].Value != null) ? player.Properties["playerFirstName"].Value.ToString() : "Player first name";
                p.playerLastName = (player.Properties["playerLastName"].Value != null) ? player.Properties["playerLastName"].Value.ToString() : "Player last name";
                p.playerNumber = (player.Properties["playerNumber"].Value != null) ? int.Parse(player.Properties["playerNumber"].Value.ToString()) : 0;
                p.teamId = t.teamId;
                p.teamName = t.teamName;
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
                c.clubId = club.Id;
                c.clubName = (club.Properties["clubName"].Value != null) ? club.Properties["clubName"].Value.ToString() : "No club name found";
                c.clubAddress = (club.Properties["clubAddress"].Value != null) ? club.Properties["clubAddress"].Value.ToString() : "No club address found";
                c.clubCity = (club.Properties["clubCity"].Value != null) ? club.Properties["clubCity"].Value.ToString() : "No club city found";
                c.clubRegion = Umbraco.GetPreValueAsString(int.Parse(club.Properties["clubRegion"].Value.ToString()));

                List<string> templist = new List<String>();
                var items = club.Properties["clubSports"].Value.ToString().Split(new Char[] { ',' });
                foreach (var item in items)
                {                    
                    int temp = int.Parse(item);
                    templist.Add(Umbraco.GetPreValueAsString(temp));
                }
                c.clubSports = templist;

                try
                {
                    c.clubLogo = apiUrl + this.getImg(club.Properties["clubLogo"].Value.ToString());
                }
                catch
                {
                    c.clubLogo = apiUrl + "/media/1001/M1.png";
                };
                
                allClubs.Add(c);
            }
            return allClubs;
        }
        //get all teams by sport with currentmatches
        public List<Team> GetTeamsBySport(int rootID)
        {
            var cs = Services.ContentService;
            List<Team> allTeams = new List<Team>();
            var teams = cs.GetById(rootID).Descendants().Where(t => t.ContentType.Alias.Equals("team"));

            foreach (var team in teams)
            {
                var t = new Team();
                t.teamId = team.Id;
                t.teamName = (team.Properties["teamName"].Value != null) ? team.Properties["teamName"].Value.ToString() : "Team name";

                var sportsType = (team.Properties["teamSport"].Value != null) ? team.Properties["teamSport"].Value.ToString() : "teamSport";
                t.teamSport = Umbraco.GetPreValueAsString(int.Parse(sportsType));

                t.teamMatches = GetMatchByTeamID(team.Id);

                allTeams.Add(t);
            }
            return allTeams;
        }
        //Get matches by team ID
        public List<Match> GetMatchByTeamID(int tID)
        {
            var cs = Services.ContentService;
            List<Match> matchlist = new List<Match>();
            var matches = cs.GetById(tID).Descendants().Where(t => t.ContentType.Alias.Equals("match"));


            foreach (var match in matches)
            {
                var m = new Match();
                m.matchId = match.Id;
                m.matchAddress = (match.Properties["matchAddress"].Value != null) ? match.Properties["matchAddress"].Value.ToString() : "Match Address";
                m.matchCity = (match.Properties["matchCity"].Value != null) ? match.Properties["matchCity"].Value.ToString() : "Match City";
                m.matchStartDateTime = (match.Properties["matchStartDateTime"].Value != null) ? match.Properties["matchStartDateTime"].Value.ToString() : "Match Start DateTime";
                m.opponent = (match.Properties["opponent"].Value != null) ? match.Properties["opponent"].Value.ToString() : "Opponent";
                m.homeGoal = int.Parse((match.Properties["homeGoal"].Value != null) ? match.Properties["homeGoal"].Value.ToString() : "0");
                m.opponentGoal = int.Parse((match.Properties["opponentGoal"].Value != null) ? match.Properties["opponentGoal"].Value.ToString() : "0");

                var matchStatus = (match.Properties["status"].Value != null) ? match.Properties["status"].Value.ToString() : "Status";
                m.status = Umbraco.GetPreValueAsString(int.Parse(matchStatus));

                matchlist.Add(m);
            }
            return matchlist;
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

            var newTeam = cs.CreateContent(data.teamName, cID, "Team");
            //NewTeam.SetValue("teamName", data.TeamName); //populate value
            cs.SaveAndPublishWithStatus(newTeam);
            Team createdTeam = new Team
            {
                teamId = newTeam.Id,
                teamName = newTeam.Name
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
                currentTeam.SetValue("teamName", data.teamName);
                currentTeam.Name = data.teamName; //the umbraco document name!!
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
                teamId = Umbraco.Content(nodeId.Id).Id,
                teamName = Umbraco.Content(nodeId.Id).Name
            };
            return team;
        }
    }
}