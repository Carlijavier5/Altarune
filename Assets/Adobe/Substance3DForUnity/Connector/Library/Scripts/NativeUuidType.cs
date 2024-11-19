using System.Runtime.InteropServices;

namespace Adobe.Substance.Connector
{
    /// <summary>
    /// Managed representation of connector uuid type
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    internal unsafe struct NativeUuidType
    {
        /// <summary>
        /// Internal array of elements. (C# uint is guaranteed to be 32-bits)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public fixed uint elements[4];
    }
}