using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserPasswordResetListModel
    {
        public List<Sky.Ramesses.DTO.UserPasswordReset> UserPasswordResetList { get; set; }

        public Sky.Ramesses.DTO.User User { get; set; }
    }
}