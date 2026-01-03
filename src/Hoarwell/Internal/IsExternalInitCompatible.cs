#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET472 || NET6_0

#pragma warning disable IDE0130
#pragma warning disable IDE0161

namespace System.Runtime.CompilerServices
{
    // Needed so we can use init setters in full fw or netstandard
    //  (details: https://developercommunity.visualstudio.com/t/error-cs0518-predefined-type-systemruntimecompiler/1244809)
    internal static class IsExternalInit { }
}

#endif
