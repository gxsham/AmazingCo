using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AmazingCo.Models;

namespace AmazingCo.Data
{
    public interface IRepository
    {
        Task<Node> GetAsync(string Id);
        Task SaveAsync(Node node);
        IQueryable<Node> GetAsync(Expression<Func<Node, bool>> predicate);
        Task<IEnumerable<Node>> GetSubtree(string parentId);
        Task<bool> ExistsAsync(string id);
        Task BulkUpdate(string newRootId);
        Task BulkSaveAsync(IEnumerable<Node> nodes);
    }
}
