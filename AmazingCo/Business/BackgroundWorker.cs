using System;
using System.Threading.Tasks;
using AmazingCo.Models;
using AmazingCo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace AmazingCo.Business
{
    public class BackgroundWorker : IBackgroundWorker
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public BackgroundWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task ChangeRoot(Node newRoot, Node node, Node parent)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            using (var context = serviceScope.ServiceProvider.GetService<ApplicationContext>())
            {
                var repository = new Repository(context);

                newRoot.ParentId = null;
                newRoot.RootId = newRoot.Id;
                newRoot.Height = 0;
                await repository.SaveAsync(newRoot);

                node.Height = parent.Height + 1;
                node.ParentId = parent.Id;
                node.RootId = newRoot.Id;
                await repository.SaveAsync(node);

                await repository.BulkUpdate(newRoot.Id);
            }
        }

        public async Task PropagateChanges(Node node, Node parent)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            using (var context = serviceScope.ServiceProvider.GetService<ApplicationContext>())
            {
                var repository = new Repository(context);

                var children = await repository.GetSubtree(node.Id);

                if (children.Select(x => x.Id).Contains(parent.Id))
                {
                    var nodeHeight = node.Height;
                    var nodeParent = node.ParentId;
                    var parentHeight = parent.Height;

                    node.Height = parentHeight - 1;
                    node.ParentId = parent.Id;

                    parent.ParentId = nodeParent;
                    parent.Height = nodeHeight;

                    await repository.SaveAsync(node);
                    await repository.SaveAsync(parent);
                    var leftChildren = children.Where(x => x.Id != node.Id && x.Id != parent.Id);

                    foreach (var item in leftChildren)
                    {
                        item.Height +=  nodeHeight - parent.Height + 1;
                    }

                    await repository.BulkSaveAsync(leftChildren);
                }
                else
                {
                    var nodeDiff = parent.Height - node.Height + 1;
                    node.Height = parent.Height + 1;
                    node.ParentId = parent.Id;

                    await repository.SaveAsync(node);

                    foreach (var item in children)
                    {
                        item.Height += nodeDiff;
                    }

                    await repository.BulkSaveAsync(children);
                }
            }
        }
    }
}
