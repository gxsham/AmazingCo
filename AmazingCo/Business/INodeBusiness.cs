using System.Linq;
using System.Threading.Tasks;
using AmazingCo.Models;

namespace AmazingCo.Business
{
    public interface INodeBusiness
    {
        IQueryable<Node> GetChildren(string parentId);
        Task ChangeParentAsync(string nodeId, string newParentId);
        Task<bool> NodeExists(string id);
    }
}
