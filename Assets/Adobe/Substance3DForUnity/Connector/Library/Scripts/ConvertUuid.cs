namespace Adobe.Substance.Connector
{
    /// <summary>
    /// Utility class for converting between uuid types.
    /// </summary>
    internal static class ConvertUuid
    {
        /// <summary>
        /// Convert a .NET uuid to a native connector uuid structure.
        /// </summary>
        /// <param name="uuid"> uuid .NET representation of a uuid. </param>
        /// <returns>  Connector native strcture representation of the uuid. </returns>
        internal static unsafe NativeUuidType ConvertToNativeUuid(System.Guid uuid)
        {
            NativeUuidType result = new NativeUuidType();

            byte[] bytes = uuid.ToByteArray();

            // The first four-byte sequence is reversed
            result.elements[0] = (uint)bytes[0] | (uint)bytes[1] << 8 |
                                 (uint)bytes[2] << 16 | (uint)bytes[3] << 24;

            // The second and third two-byte sequences are reversed
            result.elements[1] = (uint)bytes[6] | (uint)bytes[7] << 8 |
                                 (uint)bytes[4] << 16 | (uint)bytes[5] << 24;

            // The last byte sequences remain in normal ordering
            result.elements[2] = (uint)bytes[11] | (uint)bytes[10] << 8 |
                                 (uint)bytes[9] << 16 | (uint)bytes[8] << 24;
            result.elements[3] = (uint)bytes[15] | (uint)bytes[14] << 8 |
                                 (uint)bytes[13] << 16 | (uint)bytes[12] << 24;

            return result;
        }

        /// <summary>
        /// Convert a Connector native uuid struct to a .NET Guid.
        /// </summary>
        /// <param name="uuid"> uuid Native connector representation of a uuid. </param>
        /// <returns> .NET Guid representation of a uuid. </returns>
        internal static unsafe System.Guid ConvertToManagedUuid(NativeUuidType uuid)
        {
            byte[] bytes = new byte[16];

            // Reverse the first four byte sequence
            bytes[0] = (byte)(uuid.elements[0] & 0x000000ff);
            bytes[1] = (byte)((uuid.elements[0] & 0x0000ff00) >> 8);
            bytes[2] = (byte)((uuid.elements[0] & 0x00ff0000) >> 16);
            bytes[3] = (byte)((uuid.elements[0] & 0xff000000) >> 24);

            // Reverse the second and third byte sequences, each of size 2
            bytes[4] = (byte)((uuid.elements[1] & 0x00ff0000) >> 16);
            bytes[5] = (byte)((uuid.elements[1] & 0xff000000) >> 24);
            bytes[6] = (byte)(uuid.elements[1] & 0x000000ff);
            bytes[7] = (byte)((uuid.elements[1] & 0x0000ff00) >> 8);

            // The remaining byte sequences are in a normal ordering
            bytes[8] = (byte)((uuid.elements[2] & 0xff000000) >> 24);
            bytes[9] = (byte)((uuid.elements[2] & 0x00ff0000) >> 16);
            bytes[10] = (byte)((uuid.elements[2] & 0x0000ff00) >> 8);
            bytes[11] = (byte)(uuid.elements[2] & 0x000000ff);

            bytes[12] = (byte)((uuid.elements[3] & 0xff000000) >> 24);
            bytes[13] = (byte)((uuid.elements[3] & 0x00ff0000) >> 16);
            bytes[14] = (byte)((uuid.elements[3] & 0x0000ff00) >> 8);
            bytes[15] = (byte)(uuid.elements[3] & 0x000000ff);

            System.Guid result = new System.Guid(bytes);

            return result;
        }
    }
}