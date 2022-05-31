using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Raffle
{
    public class RaffleListModel
    {
        public List<Sky.Core.DTO.Raffle> RaffleList { get; set; }

        public string PageTitle { get; set; }
    }
}