namespace Game.Application.Core.Modules
{
    /// <summary>
    /// Factory abstraction for creating game modules.
    /// 
    /// This decouples GameBootstrap from concrete module classes (InputModule, AudioModule, etc.)
    /// allowing:
    /// - Easy testing (mock factory in Unit tests)
    /// - Configuration-driven module creation (JSON config later)
    /// - Runtime module swapping
    /// - Server/client module variants
    /// 
    /// SOLID: Dependency Inversion Principle
    /// Bootstrap depends on IModuleFactory (abstraction), not InputModule (concrete)
    /// </summary>
    public interface IModuleFactory 
    {
        /// <summary>
        /// Creates a module by name.
        /// Throws InvalidOperationException if module not found.
        /// </summary>
        IGameModule CreateModule(string moduleName);
    }
}
