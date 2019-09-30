using System.Threading.Tasks;
using AmazingCo.Models;

namespace AmazingCo.Business
{
    public interface IBackgroundWorker
    {
        Task PropagateChanges(Node node, int heightDifference);
    }
}
