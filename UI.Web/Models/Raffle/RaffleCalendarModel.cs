using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Raffle
{
    public class RaffleCalendarModel
    {
        public int ID { get; set; }

        public Sky.Core.DTO.Raffle Raffle { get; set; }

        public string Title { get; set; }

        public int StartedAtYear { get; set; }
        public int StartedAtMonth { get; set; }
        public int StartedAtDay { get; set; }
        public int StartedAtHour { get; set; }
        public int StartedAtMinutes { get; set; }

        public int EndedAtYear { get; set; }
        public int EndedAtMonth { get; set; }
        public int EndedAtDay { get; set; }
        public int EndedAtHour { get; set; }
        public int EndedAtMinutes { get; set; }

        public bool AllDay = true;

        public string BackgroundColor { get; set; }
    }
}