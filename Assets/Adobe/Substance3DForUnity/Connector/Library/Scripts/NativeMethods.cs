using System;
using System.Runtime.InteropServices;

namespace Adobe.Substance.Connector
{
    /// <summary>
    /// Internal delegate type to match the function pointer to pass to Connector
    /// </summary>
    /// <param name="context"></param>
    /// <param name="uuid"></param>
    /// <param name="message"></param>
    internal delegate void ConnectorTrampoline(uint context, IntPtr uuid, IntPtr message);

    /// <summary>
    /// Object maintaing bindings to the native connector library.
    /// </summary>
    internal static class NativeMethods
    {
        public const string NativeAssembly = "sbsario_connector";

        [DllImport(NativeAssembly)]
        internal extern static IntPtr RegisterConnectionEstablishedCallback(IntPtr callback);

        [DllImport(NativeAssembly)]
        internal extern static IntPtr RegisterConnectionClosedCallback(IntPtr callback);

        [DllImport(NativeAssembly)]
        internal extern static IntPtr RegisterImportAssetCallback(IntPtr callback);

        [DllImport(NativeAssembly)]
        internal extern static void ShutdownConnector();
    }
}