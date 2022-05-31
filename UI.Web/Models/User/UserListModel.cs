using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserListModel
    {
        public List<Sky.Ramesses.DTO.User> UserList { get; set; }

        public int Limit { get; set; }

        public string Query { get; set; }
    }
}