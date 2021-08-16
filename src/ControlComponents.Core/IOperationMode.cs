using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlComponents.Core
{
    public interface IOperationMode
    {
        string OpModeName { get; }
        string WORKST { get; }

        Task Select(IExecution execution, IDictionary<string, IOrderOutput> orderOutputs);
        void Deselect();
    }
}