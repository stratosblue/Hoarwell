using Hoarwell.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Hoarwell;

/// <summary>
/// 默认的 <inheritdoc cref="IHoarwellContext"/>
/// </summary>
public class HoarwellContext : IHoarwellContext
{
    #region Private 字段

    private readonly IPipeLifetimeFeature _pipeLifetimeFeature;

    private bool _isDisposed;

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public string ApplicationName { get; }

    /// <inheritdoc/>
    public CancellationToken ExecutionAborted => _pipeLifetimeFeature.PipeClosed;

    /// <inheritdoc/>
    public IFeatureCollection Features { get; }

    /// <inheritdoc/>
    public IOutputter Outputter { get; }

    /// <inheritdoc/>
    public IDictionary<object, object> Properties { get; }

    /// <inheritdoc/>
    public IServiceProvider Services { get; }

    #endregion Public 属性

    #region Protected Internal 构造函数

    /// <inheritdoc cref="HoarwellContext"/>
    protected internal HoarwellContext(string applicationName,
                                       IFeatureCollection features,
                                       IOutputter outputter)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(applicationName);
        ArgumentNullExceptionHelper.ThrowIfNull(features);
        ArgumentNullExceptionHelper.ThrowIfNull(outputter);

        ApplicationName = applicationName;
        Features = features;
        Outputter = outputter;

        Services = features.Required<IServiceProviderFeature>().Services;
        _pipeLifetimeFeature = Features.Required<IPipeLifetimeFeature>();

        //HACK 可能出现并发问题
        Properties = new Dictionary<object, object>();
    }

    #endregion Protected Internal 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public void Abort()
    {
        if (!ExecutionAborted.IsCancellationRequested)
        {
            try
            {
                _pipeLifetimeFeature.Abort();
            }
            catch
            {
                //静默异常
            }
        }
    }

    #endregion Public 方法

    #region IDisposable

    /// <summary>
    ///
    /// </summary>
    ~HoarwellContext()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            Abort();
            Outputter.Dispose();
            _isDisposed = true;
        }
    }

    #endregion IDisposable
}
