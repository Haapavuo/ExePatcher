using System;
using System.Collections.Generic;
using System.Linq;

namespace HexPatcher
{
    /// <summary>
    /// A byte array that may contain wildcard bytes instead of data.
    /// </summary>
    public class ByteFilter
    {
        /// <summary>
        /// The data bytes.
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Length of the data.
        /// </summary>
        public int Length { get { return Bytes.Length; } }

        private List<long> wildcardIndexes = new List<long>();

        /// <summary>
        /// Constructs a new ByteFilter of the given hex string.
        /// The string may contain spaces and wildcards '*'.
        /// </summary>
        public ByteFilter(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            Bytes = StringToByteArray(hexString);

            // Get all wildcards from the hex string
            for (int i = hexString.IndexOf('*'); i > -1; i = hexString.IndexOf('*', i + 1))
            {
                wildcardIndexes.Add(i / 2);
            }

            // Remove possible duplicates
            wildcardIndexes = wildcardIndexes.Distinct().ToList();
        }

        /// <summary>
        /// Returns true if the given index contains a wildcard.
        /// </summary>
        public bool IsWildcardIndex(long index)
        {
            return wildcardIndexes.Contains(index);
        }

        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        private static byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                throw new Exception("The binary key cannot have an odd number of digits");
            }

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                // Two hex characters represent one byte
                arr[i] = (byte)((GetHexValue(hex[i << 1]) << 4) + (GetHexValue(hex[(i << 1) + 1])));
            }

            return arr;
        }

        /// <summary>
        /// Converts an encoded hex character into a byte value.
        /// </summary>
        private static int GetHexValue(char hex)
        {
            if (hex == '*')
            {
                return 0;
            }

            int val = hex;

            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);

            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);

            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
