using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Raffle
{
    public class RaffleParticipantListModel
    {
        public List<Sky.Core.DTO.RaffleParticipant> RaffleParticipantList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKRaffleParticipationTypeList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKRaffleParticipationStatusList { get; set; }

        public int SelectedTKRaffleParticipationType { get; set; }

        public int SelectedTKRaffleParticipationStatus { get; set; }

        public int Limit { get; set; }
    }
}