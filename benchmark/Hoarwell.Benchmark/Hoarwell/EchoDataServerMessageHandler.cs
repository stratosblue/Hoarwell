using Microsoft.Extensions.DependencyInjection;

namespace Hoarwell.Benchmark.Hoarwell;

internal class HoarwellEchoDataServerMessageHandler : IEndpointMessageHandler<EchoData>
{
    #region Public 方法

    public Task HandleAsync(IHoarwellContext context, EchoData input)
    {
        var echo = new EchoData()
        {
            Id = input.Id * 2,
            Message = input.Message,
        };

        return context.WriteAndFlushAsync(echo, default);
    }

    #endregion Public 方法
}
