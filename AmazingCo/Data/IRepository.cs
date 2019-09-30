using System;
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
        Task<bool> ExistsAsync(string id);
    }
}
