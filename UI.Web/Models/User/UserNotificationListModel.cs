using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserNotificationListModel
    {
        public List<Sky.Ramesses.DTO.UserNotification> UserNotificationList { get; set; }

        public Sky.Ramesses.DTO.User User { get; set; }
    }
}