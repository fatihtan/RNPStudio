using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Models.Generic
{
    public class ContactMessageListModel
    {
        public List<Sky.Generic.DTO.ContactMessage> ContactMessageList { get; set; }

        public bool IsArchived { get; set; }
    }
}