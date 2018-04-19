using System;
using System.IO;

namespace HexPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var configFilePath = "config.cfg";
            if (args.Length > 0)
            {
                configFilePath = args[0];
            }

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine($"Configuration file '{configFilePath}' not found.");
                Console.WriteLine($"Working directory path: {Directory.GetCurrentDirectory()}");
            }
            else
            {
                var config = File.ReadAllLines(configFilePath);

                if (config.Length < 3)
                {
                    Console.WriteLine("Invalid config file.");
                    Console.WriteLine("Please use the following format:");
                    Console.WriteLine("BINARY FILE PATH");
                    Console.WriteLine("ORIGINAL BYTES TO FIND");
                    Console.WriteLine("MODIFIED BYTES TO REPLACE THE ORIGINAL WITH");
                }
                else
                {
                    var filePath = config[0];
                    var originalHex = config[1];
                    var modifiedHex = config[2];

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"File '{filePath}' does not exist.");
                    }
                    else
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        var originalBytes = new ByteFilter(originalHex);
                        var moddedBytes = new ByteFilter(modifiedHex);

                        // Try to find the original bytes from the file
                        var startIndex = GetStartIndex(fileBytes, originalBytes);

                        if (startIndex < 0)
                        {
                            Console.WriteLine("Original bytes were not found.");
                        }
                        else
                        {
                            // Make a backup file
                            if (SafeWriteBytesToFile(filePath + ".bak", fileBytes))
                            {
                                // Replace with the modified bytes & overwrite the original file
                                WriteBytes(fileBytes, startIndex, moddedBytes);
                                SafeWriteBytesToFile(filePath, fileBytes);
                            }
                        }
                    }
                }
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Writes the given bytes to the given file path.
        /// </summary>
        /// <returns>True if success.</returns>
        public static bool SafeWriteBytesToFile(string filePath, byte[] fileBytes)
        {
            try
            {
                File.WriteAllBytes(filePath, fileBytes);
                Console.WriteLine("Bytes written to path: " + filePath);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not write bytes to file: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Writes the bytes from the given ByteFilter into the given byte array
        /// starting at the given index.
        /// </summary>
        public static void WriteBytes(byte[] writeTo, long startIndex, ByteFilter data)
        {
            for (long i = 0; i < data.Length; i++)
            {
                if (data.IsWildcardIndex(i))
                {
                    continue;
                }
                else
                {
                    writeTo[startIndex + i] = data.Bytes[i];
                }
            }
        }

        /// <summary>
        /// Returns the start index of the first occurrence of the given ByteFilter.
        /// </summary>
        public static long GetStartIndex(byte[] fileBytes, ByteFilter findBytes)
        {
            long matchIndex = 0;
            for (long i = 0; i < fileBytes.Length; i++)
            {
                if (findBytes.IsWildcardIndex(matchIndex) || fileBytes[i] == findBytes.Bytes[matchIndex])
                {
                    matchIndex++;
                    if (matchIndex == findBytes.Length)
                    {
                        return i - matchIndex + 1;
                    }
                }
                else
                {
                    matchIndex = 0;
                }
            }
            return -1;
        }
    }
}
