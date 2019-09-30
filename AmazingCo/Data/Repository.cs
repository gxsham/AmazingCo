using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AmazingCo.Models;
using Microsoft.EntityFrameworkCore;

namespace AmazingCo.Data
{
    public class Repository : IRepository
    {
        private readonly ApplicationContext _context;

        public Repository(ApplicationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<bool> ExistsAsync(string id)
        {
            return _context.Nodes.AnyAsync(e => e.Id == id);
        }

        public Task<Node> GetAsync(string Id)
        {
            return _context.Nodes.FindAsync(Id);
        }

        public IQueryable<Node> GetAsync(Expression<Func<Node, bool>> predicate)
        {
            return _context.Nodes.Where(predicate);
        }

        public Task SaveAsync(Node node)
        {
            _context.Entry(node).State = EntityState.Modified;
            return _context.SaveChangesAsync();
        }
    }
}
