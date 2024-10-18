using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Adobe.Substance.Connector
{
    /// <summary>
    /// Maintains the interface for interacting with Connector.
    /// </summary>
    public class Instance
    {
        /// <summary>
        /// Delegate function pointer.
        /// </summary>
        private static ConnectorTrampoline OnConnectionEstablishedDelegate = OnConnectionEstablishedCallback;


        /// <summary>
        /// Delegate function pointer.
        /// </summary>
        private static ConnectorTrampoline OnConnectionClosedDelegate = OnConnectionClosedCallback;

        /// <summary>
        /// Delegate function pointer.
        /// </summary>
        private static ConnectorTrampoline OnAssetImportDelegate = OnAssetImportCallback;


        public static event EventHandler<ConnectorEventArgs> OnConnectionEstablished;
        public static event EventHandler<ConnectorEventArgs> OnConnectionClosed;
        public static event EventHandler<ConnectorEventArgs> OnImportFile;

        /// <summary>
        /// Trampoline to allow the native code to call back into C#
        /// </summary>
        /// <param name="context"> Connection context identifier that the message is from.</param>
        /// <param name="messageType"> UUID identifier for the message. </param>
        /// <param name="message"> String representing the message. </param>
        internal static void OnConnectionEstablishedCallback(uint context, IntPtr messageType, IntPtr message)
        {
            NativeUuidType nativeUuid = (NativeUuidType)Marshal.PtrToStructure(messageType, typeof(NativeUuidType));
            Guid uuid = ConvertUuid.ConvertToManagedUuid(nativeUuid);
            string convertedMessage = Marshal.PtrToStringAnsi(message);

            OnConnectionEstablished?.Invoke(null, new ConnectorEventArgs(context, convertedMessage));
        }

        internal static void OnConnectionClosedCallback(uint context, IntPtr messageType, IntPtr message)
        {
            NativeUuidType nativeUuid = (NativeUuidType)Marshal.PtrToStructure(messageType, typeof(NativeUuidType));
            Guid uuid = ConvertUuid.ConvertToManagedUuid(nativeUuid);
            string convertedMessage = Marshal.PtrToStringAnsi(message);
            OnConnectionClosed?.Invoke(null, new ConnectorEventArgs(context, convertedMessage));
        }

        internal static void OnAssetImportCallback(uint context, IntPtr messageType, IntPtr message)
        {
            NativeUuidType nativeUuid = (NativeUuidType)Marshal.PtrToStructure(messageType, typeof(NativeUuidType));
            Guid uuid = ConvertUuid.ConvertToManagedUuid(nativeUuid);
            string convertedMessage = Marshal.PtrToStringAnsi(message);
            OnImportFile?.Invoke(null, new ConnectorEventArgs(context, convertedMessage));
        }

        /// <summary>
        ///  Initialize the connector instance
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="libraryPath"></param>
        /// <returns>True on success, false on failure</returns>
        public static void Initialize()
        {
            // Register the trampoline with the native connector module
            NativeMethods.RegisterConnectionEstablishedCallback(Marshal.GetFunctionPointerForDelegate(OnConnectionEstablishedDelegate));
            
            // Register the trampoline with the native connector module
            NativeMethods.RegisterConnectionClosedCallback(Marshal.GetFunctionPointerForDelegate(OnConnectionClosedDelegate));
            
            // Register the trampoline with the native connector module
            NativeMethods.RegisterImportAssetCallback(Marshal.GetFunctionPointerForDelegate(OnAssetImportDelegate));
        }

        /// <summary>
        /// Shutdown the connector instance and library.
        /// </summary>
        /// <returns> True on success, false on failure. </returns>
        public static void Shutdown()
        {
            // Register the trampoline with the native connector module
            NativeMethods.RegisterConnectionEstablishedCallback(IntPtr.Zero);
           
            // Register the trampoline with the native connector module
            NativeMethods.RegisterConnectionClosedCallback(IntPtr.Zero);
            
            // Register the trampoline with the native connector module
            NativeMethods.RegisterImportAssetCallback(IntPtr.Zero);        
        }

        public static void SignalApplicationShutdown()
        {
            NativeMethods.ShutdownConnector();
        }
    }
}