using System;
using System.Collections.Generic;
using Game.Application.UI;
using UnityEngine;

namespace UI.Core.ViewModel
{
    
    public interface IViewModelFactory
    {
        ViewModelBase Create(Type type);
    }
    public sealed class ViewModelFactory : IViewModelFactory
    {
        private readonly Dictionary<Type, Func<ViewModelBase>> _creators = new();

        public void Register<T>(Func<T> creator)
            where T : ViewModelBase
        {
            _creators[typeof(T)] = () => creator();
        }

        public ViewModelBase Create(Type type)
        {
            if (_creators.TryGetValue(type, out var creator))
                return creator();
            var instance = Activator.CreateInstance(type);

            if (instance is not ViewModelBase vm)
                throw new InvalidOperationException($"Invalid ViewModel: {type.Name}");

            return vm;
        }

    }
}