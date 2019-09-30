using System;
using System.Threading.Tasks;
using AmazingCo.Data;
using AmazingCo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AmazingCo.Business
{
    public class BackgroundWorker : IBackgroundWorker
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public BackgroundWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task PropagateChanges(Node node, int heightDifference)
        {
            try
            {
                using (var serviceScope = _scopeFactory.CreateScope())
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationContext>())
                {
                    var repository = new Repository(context);

                    var children = await repository.GetAsync(x => x.ParentId == node.Id).ToListAsync();
                    if (children.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        foreach (var childNode in children)
                        {
                            childNode.Height -= heightDifference;
                            await repository.SaveAsync(childNode);

                            await PropagateChanges(childNode, heightDifference);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
