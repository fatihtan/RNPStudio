using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Customer
{
    public class CustomerNoteListModel
    {
        public List<Sky.Core.DTO.CustomerNote> CustomerNoteList { get; set; }

        public Sky.Core.DTO.Customer Customer { get; set; }
    }
}