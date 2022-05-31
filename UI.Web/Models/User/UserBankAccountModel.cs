using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserBankAccountModel
    {
        public Sky.Ramesses.DTO.UserBankAccount UserBankAccount { get; set; }

        public Sky.Ramesses.DTO.User User { get; set; }
    }
}