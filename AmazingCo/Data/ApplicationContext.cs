using System.Linq;
using AmazingCo.Models;
using Microsoft.EntityFrameworkCore;

namespace AmazingCo.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Node> Nodes { get; set; }

        public void Seed()
        {
            if (Nodes.Any())
            {
                return;
            }

            var rootId = "root";
            var root = new Node
            {
                Height = 0,
                Id = rootId,
                RootId = rootId
            };

            Nodes.Add(root);

            var aNode = new Node
            {
                Height = 1,
                ParentId = rootId,
                RootId = rootId,
                Id = "a"
            };
            var bNode = new Node
            {
                Height = 1,
                ParentId = rootId,
                RootId = rootId,
                Id = "b"
            };

            Nodes.AddRange(aNode, bNode);

            var cNode = new Node
            {
                Height = 2,
                ParentId = aNode.Id,
                RootId = rootId,
                Id = "c"
            };

            Nodes.Add(cNode);

            var dNode = new Node
            {
                Height = 3,
                ParentId = cNode.Id,
                RootId = rootId,
                Id = "d"
            };

            var eNode = new Node
            {
                Height = 3,
                ParentId = cNode.Id,
                RootId = rootId,
                Id = "e"
            };

            Nodes.AddRange(dNode, eNode);

            SaveChanges();
        }
    }
}
