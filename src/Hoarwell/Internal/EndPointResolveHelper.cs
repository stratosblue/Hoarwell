using System.Net;

namespace Hoarwell.Internal;

/// <summary>
/// 端点处理帮助类
/// </summary>
internal static class EndPointResolveHelper
{
    #region Public 方法

    /// <summary>
    /// 处理为Socket可用端点
    /// </summary>
    /// <param name="endPoint"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<EndPoint> ResolveForSocketAsync(EndPoint endPoint, CancellationToken cancellationToken = default)
    {
        //将dns转换为IP地址
        if (endPoint is DnsEndPoint dnsEndPoint)
        {
            var addresses =
#if NET8_0_OR_GREATER
                await Dns.GetHostAddressesAsync(dnsEndPoint.Host, dnsEndPoint.AddressFamily, cancellationToken).ConfigureAwait(false);
#else
                await Dns.GetHostAddressesAsync(dnsEndPoint.Host).ConfigureAwait(false);
#endif
            //随机取一个？
            return new IPEndPoint(addresses.OrderBy(m => Guid.NewGuid()).First(), dnsEndPoint.Port);
        }

        //其它场景需要丰富

        return endPoint;
    }

    #endregion Public 方法
}
