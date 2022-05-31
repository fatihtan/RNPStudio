using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Home
{
    public class HomeListModel
    {
        //  Ramesses
        public int TotalUserCount { get; set; }


        //  Core
        public decimal TotalCustomerCurrentBalance { get; set; }

        //  Core
        public int TotalCustomerCount { get; set; }

        //  Core
        public int TotalPaymentCount { get; set; }

        //  Core
        public int TotalRaffleParticipantCount { get; set; }

        //  Core
        public int PendingRaffleParticipantCount { get; set; }

        //  Core
        public int TotalRaffleCount { get; set; }



        //  Generic
        public int ContactMessageCount { get; set; }

        //  Log
        public int LogUserSessionCount { get; set; }

        //  Log
        public int LogNavigationCount { get; set; }

        //  Log
        public int LogNavigationAdministratorCount { get; set; }

        //  Log
        public int LogURLErrorCount { get; set; }


        public List<Sky.Common.DTO.ActivityLog> ActivityLogListForLeft { get; set; }

        public List<Sky.Common.DTO.ActivityLog> ActivityLogListForRight { get; set; }
    }
}