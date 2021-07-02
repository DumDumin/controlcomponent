using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlComponents.Core
{
    public interface IControlComponentProvider
    {
        T GetComponent<T>(string id);
        IEnumerable<T> GetComponents<T>();
        int CountComponents<T>();
    }

    public class ControlComponentProvider : Dictionary<string, IControlComponent>, IControlComponentProvider
    {
        public T GetComponent<T>(string id)
        {
            return (T)this.Values.First(c => c.ComponentName == id && c is T);
        }

        public int CountComponents<T>()
        {
            return this.Values.Count(c => c is T);
        }

        public IEnumerable<T> GetComponents<T>()
        {
            return this.Values.Where(c => c is T).Select(c => (T)c);
        }
    }
}