using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserPasswordListModel
    {
        public List<Sky.Ramesses.DTO.UserPasswordInfo> UserPasswordInfoList { get; set; }

        public Sky.Ramesses.DTO.User User { get; set; }
    }
}