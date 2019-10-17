using System;
using System.Linq;
using System.Threading.Tasks;
using AmazingCo.Models;
using AmazingCo.Data;
using Microsoft.EntityFrameworkCore;

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
            if (nodeId == null)
            {
                throw new ArgumentNullException(nameof(nodeId));
            }
            if (newParentId == null)
            {
                throw new ArgumentNullException(nameof(newParentId));
            }

            var node = await _repository.GetAsync(nodeId);
            if (node == null)
            {
                throw new ArgumentException(nameof(node));
            }

            var parent = await _repository.GetAsync(newParentId);
            if (parent == null)
            {
                throw new ArgumentException(nameof(parent));
            }

            if (nodeId == newParentId)
            {
                return;
            }

            //handling situation when there is a new root node
            if (node.ParentId == null)
            {
                var children = _repository.GetAsync(x => x.ParentId == node.Id).ToList();
                if (children.Count != 1)
                {
                    throw new ArgumentException($"Root should have only one child to be replaced.");
                }

                var newRoot = children.First();

                Task.Run(async () => await _worker.ChangeRoot(newRoot, node, parent));
            }
            else
            {
                Task.Run(async () => await _worker.PropagateChanges(node, parent));
            }
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
