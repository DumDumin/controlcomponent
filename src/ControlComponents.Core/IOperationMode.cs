using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlComponents.Core
{
    public interface IOperationMode
    {
        string OpModeName { get; }
        string WORKST { get; }

        // Parameters are needed, because an operationmode can be added to a controlcomponent after its creation
        Task Select(IExecution execution, IDictionary<string, IOrderOutput> orderOutputs);
        void Deselect();
    }
}