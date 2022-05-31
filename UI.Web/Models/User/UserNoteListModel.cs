using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.User
{
    public class UserNoteListModel
    {
        public List<Sky.Ramesses.DTO.UserNote> UserNoteList { get; set; }

        public Sky.Ramesses.DTO.User User { get; set; }
    }
}