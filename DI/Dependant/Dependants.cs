using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using DIFeatures.Dependant.Comparison;
using DIFeatures.Dependency;
using DIFeatures.Public.Extensions;
using DIFeatures.Registered;

namespace DIFeatures.Dependant
{
  internal class Dependants : IDependants
  {
    private readonly Dependencies _dependencies;
    private readonly HashSet<object> _dependants;

    public Dependants()
    {
      _dependencies = new Dependencies();
      _dependants = new HashSet<object>(Compared.ByReference());
    }

    public void Inject(object instance, IFeatures features)
    {
      if(!_dependants.Add(instance))
        throw new InvalidOperationException($"{instance.Type().Name} injected already.");
      
      _dependencies
        .Of(instance.Type())
        .Within(instance)
        .InjectWith(features);
    }

    public void Release(object instance)
    {
      if(!_dependants.Remove(instance))
        throw new InvalidOperationException($"{instance.Type().Name} released already.");
      
      _dependencies
        .Of(instance.Type())
        .Within(instance)
        .Release();
    }

    public void ReleaseAll()
    {
      if (_dependants.Count > 0)
      {
        var leaked = _dependants.ToArray();
        leaked.ForEach(x => x.ReleaseDependencies());
      }
      
      _dependants.Clear();
    }
  }
}