using System;
using Microsoft.SemanticKernel;

namespace MultiAgentDemo.Services
{
    /// <summary>
    /// Provides a strongly-typed Semantic Kernel instance.
    /// Implementations receive the Kernel via DI so callers can depend on the public SK API.
    /// </summary>
    public interface ISemanticKernelProvider
    {
        /// <summary>
        /// Returns the injected <see cref="Kernel"/> instance or null if unavailable.
        /// </summary>
        Kernel? GetKernel();
    }
}
