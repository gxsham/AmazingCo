using System;
using System.Collections.Generic;
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

        public Task BulkUpdate(string newRootId)
        {
            var nodes = _context.Nodes.Where(x => x.RootId != newRootId);
            foreach (var node in nodes)
            {
                node.RootId = newRootId;
                node.Height -= 1;
                _context.Entry(node).State = EntityState.Modified;
            }
            return _context.SaveChangesAsync();
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

        public async Task<IEnumerable<Node>> GetSubtree(string parentId)
        {
            var list = await _context.Nodes
                .Where(x => x.ParentId == parentId)
                .AsNoTracking()
                .ToListAsync();

            var result = new List<Node>();
            result.AddRange(list);

            foreach (var item in list)
            {
                await GetNodes(result, item.Id);
            }

            return result;
        }

        private async Task GetNodes(List<Node> fullList, string parentId)
        {
            var children = await _context.Nodes
                .Where(x => x.ParentId == parentId)
                .AsNoTracking()
                .ToListAsync();

            if (children.Count > 0)
            {
                fullList.AddRange(children);
                foreach (var item in children)
                {
                    await GetNodes(fullList, item.Id);
                }
            }
            return; 
        }

        public Task SaveAsync(Node node)
        {
            try
            {
                _context.Entry(node).State = EntityState.Modified;
                return _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Task BulkSaveAsync(IEnumerable<Node> nodes)
        {
                foreach (var node in nodes)
                {
                    _context.Entry(node).State = EntityState.Modified;
                }

            return _context.SaveChangesAsync();
        }
    }
}
