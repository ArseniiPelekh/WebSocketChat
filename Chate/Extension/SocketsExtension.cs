using Chate.Handlers;
using Chate.SocketsManager;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Chate.Extension
{
    public static class SocketsExtension {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services) {
            services.AddTransient<ConnectionManager>();
            foreach (var type in Assembly.GetExecutingAssembly().ExportedTypes) {
                if (type.GetTypeInfo().BaseType == typeof(SocketHandler)) {
                    services.AddSingleton(type);
                }
            }
            return services;
        }
    }
}