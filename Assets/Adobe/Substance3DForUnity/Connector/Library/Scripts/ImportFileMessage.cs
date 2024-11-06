using System;

namespace Adobe.Substance.Connector
{
    /// <summary>
    /// Class for representing the JSON message from a import callback.
    /// </summary>
    [Serializable]
    public class ImportFileMessage
    {
        public string name;
        public string path;
        public bool take_file_ownership;
        public string type;
        public string uuid;
    }

    public class ConnectorEventArgs : EventArgs
    {
        public uint Context { get; }

        public string Message { get; }

        public ConnectorEventArgs(uint context, string message)
        {
            Context = context;
            Message = message;  
        }
    }
}
