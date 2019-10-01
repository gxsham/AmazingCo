using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AmazingCo.ApiGW.Clients
{
    public class NodeClient : INodeClient
    {
        private readonly HttpClient _httpClient;

        public NodeClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Task<HttpResponseMessage> ChangeParentAsync(string nodeId, string parentId)
        {
            return _httpClient.PutAsync($"api/nodes/{nodeId}?parentId={parentId}", null);
        }

        public Task<HttpResponseMessage> GetChildrenAsync(string parentId)
        {
            return _httpClient.GetAsync($"api/nodes/{parentId}/getchildren");
        }
    }
}
