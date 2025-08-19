using System;
using System.Linq;

namespace MultiAgentDemo.Services
{
    // Hosted service that uses reflection to find an IKernel instance from DI and invoke
    // any available Azure OpenAI extension method exposed by the Semantic Kernel connectors.
    internal sealed class KernelAzureOpenAIConfigurator : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly IServiceProvider _sp;
        private readonly string _connectionString;

        public KernelAzureOpenAIConfigurator(IServiceProvider sp, string connectionString)
        {
            _sp = sp;
            _connectionString = connectionString;
        }

        public System.Threading.Tasks.Task StartAsync(System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                // Try to locate the IKernel type by name from loaded assemblies
                var ikernelType = Type.GetType("Microsoft.SemanticKernel.IKernel, Microsoft.SemanticKernel")
                                 ?? AppDomain.CurrentDomain.GetAssemblies()
                                    .Select(a => a.GetType("Microsoft.SemanticKernel.IKernel"))
                                    .FirstOrDefault(t => t != null);

                if (ikernelType == null) return System.Threading.Tasks.Task.CompletedTask;

                var kernel = _sp.GetService(ikernelType);
                if (kernel == null) return System.Threading.Tasks.Task.CompletedTask;

                // Search loaded assemblies for a public static extension method that looks like
                // UseAzureOpenAI / AddAzureOpenAI / WithAzureOpenAI and accepts (IKernel, string)
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        var types = asm.GetTypes();
                        foreach (var t in types)
                        {
                            if (!t.IsSealed || !t.IsAbstract) continue; // static class
                            var methods = t.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                            foreach (var m in methods)
                            {
                                if (!(m.Name.Contains("UseAzureOpenAI") || m.Name.Contains("AddAzureOpenAI") || m.Name.Contains("WithAzureOpenAI"))) continue;
                                var pars = m.GetParameters();
                                if (pars.Length < 2) continue;
                                if (!pars[0].ParameterType.IsAssignableFrom(ikernelType)) continue;

                                try
                                {
                                    // Invoke extension method as: Extension(kernel, connectionString)
                                    m.Invoke(null, new object[] { kernel, _connectionString });
                                    return System.Threading.Tasks.Task.CompletedTask;
                                }
                                catch
                                {
                                    // ignore and continue searching
                                }
                            }
                        }
                    }
                    catch
                    {
                        // ignore assembly load/type inspection errors
                    }
                }
            }
            catch
            {
                // non-fatal
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public System.Threading.Tasks.Task StopAsync(System.Threading.CancellationToken cancellationToken) => System.Threading.Tasks.Task.CompletedTask;
    }
}
