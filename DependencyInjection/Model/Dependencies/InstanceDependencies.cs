using System.Reflection;
using DependencyInjection.Core;
using DependencyInjection.Model.Dependencies.Extensions;

namespace DependencyInjection.Model.Dependencies
{
  internal struct InstanceDependencies
  {
    private readonly object _instance;
    private readonly FieldInfo[] _dependencies;

    public InstanceDependencies(object instance, FieldInfo[] dependencies) =>
      (_instance, _dependencies) =
      (instance, dependencies);
    
    public void InjectWith(IFeatures features)
    {
      foreach (var dependency in _dependencies)
      {
        var feature = features.ImplementationOf(dependency.Type());

        dependency
         .Of(_instance)
         .InjectWith(feature);
      }
    }

    public void Release()
    {
      foreach (var dependency in _dependencies)
        dependency
         .Of(_instance)
         .Release();
    }
  }
}