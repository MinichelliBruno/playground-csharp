// ReflectionLab/IsExternalInit.cs
#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    // Polyfill para permitir 'init' y records en netstandard2.1
    internal static class IsExternalInit { }
}
#endif