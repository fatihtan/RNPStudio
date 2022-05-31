using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Statistics
{
    public class GraphicMonitorModel
    {
        public List<Sky.CMS.DTO.TypeValue> TKLogList { get; set; }

        public List<Sky.CMS.DTO.TypeValue> TKEntityList { get; set; }
    }
}