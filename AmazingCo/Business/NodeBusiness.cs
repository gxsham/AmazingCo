using System;
using System.Linq;
using System.Threading.Tasks;
using AmazingCo.Data;
using AmazingCo.Models;

namespace AmazingCo.Business
{
    public class NodeBusiness : INodeBusiness
    {
        private readonly IRepository _repository;
        private readonly IBackgroundWorker _worker;

        public NodeBusiness(IRepository repository, IBackgroundWorker worker)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _worker = worker ?? throw new ArgumentNullException(nameof(worker));
        }
        public async Task ChangeParentAsync(string nodeId, string newParentId)
        {
            var node = await _repository.GetAsync(nodeId);
            var parent = await _repository.GetAsync(newParentId);
            node.ParentId = parent.Id;
            var heightDiff = node.Height - parent.Height - 1;
            node.Height = parent.Height + 1;

            await _repository.SaveAsync(node);

            Task.Run(async () => await _worker.PropagateChanges(node, heightDiff));
        }

        public IQueryable<Node> GetChildren(string parentId)
        {
            return _repository.GetAsync(x => x.ParentId == parentId);
        }

        public Task<bool> NodeExists(string id)
        {
            return _repository.ExistsAsync(id);
        }
    }
}
