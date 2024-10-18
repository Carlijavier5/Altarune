using System;

namespace Adobe.Substance.Connector
{
    public static class GUID
    {
        /// <summary>
        /// UUID representing the connection established message.
        /// </summary>
        public readonly static Guid ConnectionEstablishedUuid = new Guid("02572bc5-2d84-450a-9e01-d22c66b1abb1");

        /// <summary>
        /// UUID representing the connection closed message
        /// </summary>
        public readonly static Guid ConnectionClosedUuid = new Guid("04705ddf-16d4-4489-af6c-6e3a93f1959d");

        /// <summary>
        /// UUID representing the sbsar import operation
        /// </summary>
        public readonly static Guid ImportSbsarUuid = new Guid("91e3dfbc-80b8-4b1a-92d5-63ec09ac641a");
    }
}
