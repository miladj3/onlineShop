using System.Collections.Generic;
using onlineShop.Models;

namespace onlineShop.Services
{
    public interface IAuditTrailService
    {
        List<ChangeLog> RetrieveAndLogChanges();
    }
}