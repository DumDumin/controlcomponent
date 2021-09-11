using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlComponents.Core
{
    public interface IControlComponentProvider
    {
        T GetComponent<T>(string id) where T : IControlComponent;
    }

    // TODO there can be also providers to search the network and create network cc to access the real one (factory)
    // => http://wiki.eclipse.org/BaSyx_/_Documentation_/_Components_/_Registry
    public class ControlComponentProvider : Dictionary<string, IControlComponent>, IControlComponentProvider
    {
        // TODO get IControlComponent from dict and check type
        public T GetComponent<T>(string id) where T : IControlComponent
        {
            try
            {
                return (T)this.Values.First(c => c.ComponentName == id);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException($"Cannot cast {id} to {typeof(T)}", e);
            }
        }
    }
}