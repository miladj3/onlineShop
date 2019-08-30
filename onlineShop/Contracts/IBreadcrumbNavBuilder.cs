using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace onlineShop.Contracts
{
    public interface IBreadcrumbNavBuilder
    {
        void CreateForNode(string currentNodeName, object BNBData, Controller controller);
    }
}