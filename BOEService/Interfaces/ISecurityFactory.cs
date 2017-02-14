using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOEService.Entites.BOE;
using BOEService.Models;

namespace BOEService.Interfaces
{
    public interface ISecurityFactory
    {
        LogInStatus CheckLogIn(LogOnModel _Entity);
        dynamic PagePermissedList(int userGroupID);
        PagePermissionVM GetCRUDPermission(int userGroupID, string pageName);
    }
}
