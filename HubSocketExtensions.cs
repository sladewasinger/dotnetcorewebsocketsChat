﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace HubSockets
{
    public static class HubSocketExtensions
    {
        public static void UseHubSockets(this IApplicationBuilder app, HubSocketAcceptor socketAcceptor, params Assembly[] assemblies)
        {
            socketAcceptor.RegisterHubTypes(assemblies);

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(60),
                ReceiveBufferSize = 16 * 1024
            };

            app.Map("/ws", a => {
                a.UseWebSockets(webSocketOptions);
                a.Use(socketAcceptor.SocketAcceptor);
            });
        }

        public static void RegisterHubSockets(this IServiceCollection services)
        {
            services.AddSingleton<HubSocketAcceptor>();
            services.AddSingleton<HubSocketRepository>();
            services.AddSingleton<HubSocketClients>();
        }

        public static void AddHub<THub>(this IServiceCollection services) where THub : Hub
        {
            services.AddTransient<THub>();
        }
    }
}
