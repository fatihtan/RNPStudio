using Sky.Common.DTO;
using Sky.SuperUser.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web.Infrastructure
{
    public class PermissionActionControl
    {
        public static bool Check(List<AdministratorPermissionAction> list, TK.Project.Studio.TKPermissionAction TKPermissionAction)
        {
            bool s = list.Any(per => per.PermissionActionValue == (int)TKPermissionAction);
            return s;
        }
    }
}