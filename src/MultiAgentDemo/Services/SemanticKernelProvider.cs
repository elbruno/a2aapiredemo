using System;
using System.Linq;
using Microsoft.SemanticKernel;

namespace MultiAgentDemo.Services
{
    /// <summary>
    /// Best-effort provider that resolves an IKernel instance from DI and attempts to
    /// attach Azure OpenAI connector using the provided connection string.
    /// </summary>
    public sealed class SemanticKernelProvider : ISemanticKernelProvider
    {
        private readonly Kernel? _kernel;

        public SemanticKernelProvider(Kernel? kernel)
        {
            _kernel = kernel;
        }

        public Kernel? GetKernel() => _kernel;
    }
}
