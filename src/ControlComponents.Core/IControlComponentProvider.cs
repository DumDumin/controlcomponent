using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlComponents.Core
{
    public interface IControlComponentProvider
    {
        T GetComponent<T>(string id) where T : IControlComponent;
        IEnumerable<T> GetComponents<T>();
    }
}