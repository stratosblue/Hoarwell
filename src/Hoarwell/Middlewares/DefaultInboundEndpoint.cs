﻿using System.Collections.Frozen;
using Hoarwell.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hoarwell.Middlewares;

internal sealed class DefaultInboundEndpoint<TContext>
    : PipelineEndpoint<TContext, InboundMetadata>
    where TContext : IHoarwellContext
{
    #region Private 字段

    private readonly bool _closePipeOnMessageHandleException;

    private readonly FrozenDictionary<Type, HandleInboundMessageDelegate> _handleInboundMessageDelegateMap;

    private readonly bool _handleMessageAsynchronously;

    private readonly ILogger _logger;

    private readonly HoarwellOptions _options;

    #endregion Private 字段

    #region Public 构造函数

    public DefaultInboundEndpoint([ServiceKey] string applicationName,
                                  IOptionsMonitor<HoarwellOptions> optionsMonitor,
                                  IOptionsMonitor<DefaultInboundMessageHandleOptions> messageHandleOptionsMonitor,
                                  ILogger<DefaultInboundEndpoint<TContext>> logger)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);
        ArgumentNullExceptionHelper.ThrowIfNull(optionsMonitor);
        ArgumentNullExceptionHelper.ThrowIfNull(messageHandleOptionsMonitor);
        ArgumentNullExceptionHelper.ThrowIfNull(logger);

        _handleInboundMessageDelegateMap = messageHandleOptionsMonitor.GetRequiredApplicationOptions(applicationName, options => options.HandleInboundMessageDelegateMap)
                                                                      .ToFrozenDictionary();

        _options = optionsMonitor.Get(applicationName);

        _handleMessageAsynchronously = _options.HandleMessageAsynchronously;
        _closePipeOnMessageHandleException = _options.ClosePipeOnMessageHandleException;

        _logger = logger;
    }

    #endregion Public 构造函数

    #region Public 方法

    public override Task InvokeAsync(TContext context, InboundMetadata input)
    {
        var processTask = InternalInvokeAsync(context, input);

        return _handleMessageAsynchronously
               ? Task.CompletedTask
               : processTask;
    }

    #endregion Public 方法

    #region Private 方法

    private async Task InternalInvokeAsync(TContext context, InboundMetadata input)
    {
        try
        {
            await _handleInboundMessageDelegateMap[input.ValueType](context, input).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the message");

            if (_closePipeOnMessageHandleException)
            {
                try
                {
                    context.Abort();
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, "An error occurred while close pipe");
                }
            }
        }
    }

    #endregion Private 方法
}
