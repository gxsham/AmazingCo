using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AmazingCo.ApiGW.Clients
{
    public interface INodeClient
    {
        Task<HttpResponseMessage> GetChildrenAsync(string parentId);
        Task<HttpResponseMessage> ChangeParentAsync(string nodeId, string parentId);
    }
}
