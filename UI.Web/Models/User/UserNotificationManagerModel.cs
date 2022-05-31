using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserNotificationManagerModel
    {
        public List<Sky.Ramesses.DTO.User> UserList { get; set; }

        public List<Sky.Ramesses.DTO.UserTeam> UserTeamList { get; set; }
    }
}