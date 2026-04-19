using System.Collections.Generic;
using Cysharp.Threading.Tasks;
namespace Game.Application.Configuration.Abstractions
{
    /// <summary>
    /// Provides configuration data from a specific source.
    /// </summary>
    public interface IConfigProvider
    {
        UniTask<IEnumerable<T>> LoadConfigsAsync<T>() where T : IConfig;
        
    }

}
