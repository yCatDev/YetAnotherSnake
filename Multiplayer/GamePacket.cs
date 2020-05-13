using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace YetAnotherSnake.Multiplayer
{
    [Serializable]
    public class GamePacket
    {
        public int Id;
        public int[] idsToCreate;
        
        public bool Disconnect = false;

        public bool StartGame = false;

        public bool Test = false;
        
        public bool ServiceData = false;
        public bool LeftKeyDown;
        public bool RightKeyDown;
        

        // Convert an object to a byte array
        public static byte[] GetBytes(GamePacket obj)
        {
            if (obj == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(memoryStream, obj);

                var compressed = Compress(memoryStream.ToArray());
                return compressed;
            }
        }

// Convert a byte array to an Object
        public static GamePacket FromBytes(byte[] arrBytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();               
                var decompressed = Decompress(arrBytes);
 
                memoryStream.Write(decompressed, 0, decompressed.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
 
                return (GamePacket) binaryFormatter.Deserialize(memoryStream);                
            }     
        }

        private static byte[] Compress(byte[] input)
        {
            byte[] compressesData;

            using (var outputStream = new MemoryStream())
            {
                using (var zip = new GZipStream(outputStream, CompressionLevel.Fastest, true))
                {
                    zip.Write(input, 0, input.Length);
                }

                compressesData = outputStream.ToArray();
            }

            return compressesData;
        }

        private static byte[] Decompress(byte[] input)
        {
            using var outputStream = new MemoryStream();
            using (var inputStream = new MemoryStream(input))
            {
                using (var zip = new GZipStream(inputStream, CompressionMode.Decompress, true))
                {
                    zip.CopyTo(outputStream);
                }
            }

            var decompressedData = outputStream.ToArray();

            return decompressedData;
        }
    }
}
