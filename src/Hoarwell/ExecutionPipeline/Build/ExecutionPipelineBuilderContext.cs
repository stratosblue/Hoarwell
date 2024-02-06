namespace Hoarwell.ExecutionPipeline.Build;

internal sealed class ExecutionPipelineBuilderContext
{
    #region Private 字段

    private Func<object?, object>? _pipelineBuildDelegate;

    private int _version = 0;

    #endregion Private 字段

    #region Public 属性

    public int Version => _version;

    #endregion Public 属性

    #region Internal 属性

    internal Func<object?, object>? PipelineBuildDelegate => _pipelineBuildDelegate;

    #endregion Internal 属性

    #region Public 方法

    public void Update(Func<object?, object> pipelineBuildDelegate, int checkVersion)
    {
        if (checkVersion != _version)
        {
            throw new InvalidOperationException("The pipeline builder has changed");
        }

        _version++;

        _pipelineBuildDelegate = pipelineBuildDelegate;
    }

    #endregion Public 方法
}
