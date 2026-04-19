using System.Collections.Generic;
using UnityEngine;
namespace Game.Application.Configuration.Abstractions
{
    interface IConfigCollection
    {
        
    }

    class ConfigCollection<T> : IConfigCollection
    {
        //public Dictionary<ConfigId<T>, T> Data;
    }
}