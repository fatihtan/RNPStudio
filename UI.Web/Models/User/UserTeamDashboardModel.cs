using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserTeamDashboardModel
    {
        public List<Sky.Ramesses.DTO.UserTeam> UserTeamList { get; set; }

        public List<Sky.Core.DTO.Payment> PaymentList { get; set; }

        public List<Sky.Core.DTO.Payment> ConfirmedPaymentList { get; set; }

        public List<Sky.Core.DTO.Customer> CustomerList { get; set; }

        public Sky.Ramesses.DTO.UserTeam SelectedUserTeam { get; set; }

        public List<Sky.Ramesses.DTO.User> UserList { get; set; }

        public decimal TotalUserTeamBalance { get; set; }

        public bool IsUserTeamSelected { get; set; }

        public int SelectedUserTeamID { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public bool IncludeLeader { get; set; }
    }
}