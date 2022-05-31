using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Raffle
{
    public class RaffleModel
    {
        public Sky.Core.DTO.Raffle Raffle { get; set; }

        public Sky.Core.DTO.RaffleTemplate RaffleTemplate { get; set; }

        public List<Sky.Core.DTO.RaffleParticipant> RaffleParticipantList { get; set; }

        public List<Sky.Core.DTO.Customer> CustomerList { get; set; }

        public System.Globalization.CultureInfo TurkishCultureInfo { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKRaffleStatusList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKRaffleParticipationStatusList { get; set; }
    }
}