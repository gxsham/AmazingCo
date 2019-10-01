using System.Threading.Tasks;
using AmazingCo.Models;

namespace AmazingCo.Business
{
    public interface IBackgroundWorker
    {
        Task PropagateChanges(Node node, Node parent);
        Task ChangeRoot(Node root, Node node, Node parent);
    }
}
