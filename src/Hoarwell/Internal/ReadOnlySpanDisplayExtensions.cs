using System.Text;

namespace Hoarwell.Internal;

internal static class ReadOnlySpanDisplayExtensions
{
    #region Public 方法

    public static string ToDisplay<T>(this ReadOnlySpan<T> values)
    {
        if (values.IsEmpty)
        {
            return string.Empty;
        }
        var builder = new StringBuilder(values.Length * 4);
        foreach (var v in values)
        {
            builder.Append(v?.ToString());
            builder.Append(',');
        }
        if (builder.Length > 0)
        {
            builder.Remove(builder.Length - 1, 1);
        }
        return builder.ToString();
    }

    #endregion Public 方法
}
