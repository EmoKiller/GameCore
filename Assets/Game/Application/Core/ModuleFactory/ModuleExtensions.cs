using Game.Application.Core;
using Game.Application.Core.Modules;
using UnityEngine;

public static class ModuleExtensions 
{
    public static void AddModule<T>(this ModuleFactory factory, GameApplication app) where T : IGameModule, new()
    {
        string moduleName = typeof(T).Name; // Tự động lấy tên Class làm Key
        factory.RegisterModule(moduleName, () => new T());
        app.RegisterModule(factory.CreateModule(moduleName));
    }
}
