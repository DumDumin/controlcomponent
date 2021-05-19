using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlComponents.Core
{
    public interface IOperationMode
    {
        string OpModeName { get; }

        Task Select(IExecution execution, IDictionary<string, OrderOutput> orderOutputs);
        void Deselect();
    }
}