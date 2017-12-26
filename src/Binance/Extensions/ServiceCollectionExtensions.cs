﻿using System;
using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBinance(this IServiceCollection services)
        {
            // API
            services.AddSingleton<IBinanceApiUserProvider, BinanceApiUserProvider>();
            services.AddSingleton<IBinanceHttpClient>(s =>
            {
                if (!BinanceHttpClient.Initializer.IsValueCreated)
                {
                    // Replace initializer.
                    BinanceHttpClient.Initializer = new Lazy<BinanceHttpClient>(() =>
                    {
                        return new BinanceHttpClient(
                            s.GetService<IApiRateLimiter>(),
                            s.GetService<IOptions<BinanceApiOptions>>(),
                            s.GetService<ILogger<BinanceHttpClient>>());
                    }, true);
                }

                return BinanceHttpClient.Instance;
            });
            services.AddTransient<IApiRateLimiter, ApiRateLimiter>();
            services.AddTransient<IRateLimiter, RateLimiter>();
            services.AddSingleton<IBinanceApi, BinanceApi>();

            // WebSocket
            services.AddTransient<IWebSocketClient, WebSocketClient>();

            // Cache
            services.AddTransient<ITradeCache, TradeCache>();
            services.AddTransient<IOrderBookCache, OrderBookCache>();
            services.AddTransient<IAccountInfoCache, AccountInfoCache>();
            services.AddTransient<ICandlestickCache, CandlestickCache>();
            services.AddTransient<IAggregateTradeCache, AggregateTradeCache>();

            // WebSockets
            services.AddTransient<ITradeWebSocketClient, TradeWebSocketClient>();
            services.AddTransient<IDepthWebSocketClient, DepthWebSocketClient>();
            services.AddTransient<ICandlestickWebSocketClient, CandlestickWebSocketClient>();
            services.AddTransient<IAggregateTradeWebSocketClient, AggregateTradeWebSocketClient>();
            services.AddTransient<IUserDataWebSocketClient, UserDataWebSocketClient>();

            return services;
        }
    }
}
