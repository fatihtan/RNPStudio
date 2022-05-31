using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Statistics
{
    public class LogListModel
    {
        public List<Sky.Log.DTO.BaseDTO> LogList { get; set; }


        public List<Sky.CMS.DTO.TypeValue> TKLogList { get; set; }

        public Sky.Common.DTO.TK.TKLog SelectedTKLog { get; set; }

        public bool IsTKSelected { get; set; }

        public int SelectedTKID { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public int? UserID { get; set; }

        public Sky.Ramesses.DTO.User User { get; set; }

        public int? AdministratorID { get; set; }

        public Sky.SuperUser.DTO.Administrator Administrator { get; set; }
    }
}