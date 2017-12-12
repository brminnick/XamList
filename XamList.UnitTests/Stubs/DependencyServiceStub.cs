using System;
using System.Collections.Generic;

namespace XamList.UnitTests
{
    public class DependencyServiceStub : IDependencyService
    {
        #region Constant Fields
        readonly Dictionary<Type, object> _registeredServices = new Dictionary<Type, object>();
        #endregion

        #region Methods
        public void Register<T>(object impl) => _registeredServices[typeof(T)] = impl;

        public T Get<T>() where T : class => (T)_registeredServices[typeof(T)];
        #endregion
    }
}