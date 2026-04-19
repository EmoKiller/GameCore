


using System.Collections.Generic;

namespace Game.Application.Configuration.Abstractions
{
    public interface IConfigValidator<T> where T : IConfig
    {
        void Validate(T config, IReadOnlyCollection<T> allConfigs);
    }
}