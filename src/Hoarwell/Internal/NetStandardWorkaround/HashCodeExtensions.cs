namespace System;

internal static class HashCodeExtensions
{
    #region Public 方法

    public static void AddBytes(this HashCode hashCode, ReadOnlySpan<byte> value)
    {
        int hash = 0;
        foreach (byte b in value)
        {
            hash ^= b;
        }
        hashCode.Add(hash);
    }

    #endregion Public 方法
}
