using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;

namespace YetAnotherSnake.Multiplayer
{

    [Serializable]
    public enum Protocol
    {
        None = 0, Start = 1, Disconnect = 2, MoveSnake = 3, SpawnFood = 4, Pause = 5,
    }
    
    [Serializable]
    public class GamePacket
    {
        private List<KeyValuePair<Protocol,IPacket>> _packets = new List<KeyValuePair<Protocol,IPacket>>();

        private int _counter = 0;
        
        public delegate void StartGameReceived(StartGamePacket received);
        public delegate void DisconnectGameReceived(DisconnectGamePacket received);

        public delegate void MoveSnakeReceived(MoveSnakePacket received);
        public delegate void SpawnFoodReceived(SpawnFoodPacket received);
        public delegate void PauseReceived(PauseGamePacket received);

        public event StartGameReceived OnStartGameReceived;
        public event DisconnectGameReceived OnDisconnectGameReceived;
        public event MoveSnakeReceived OnMoveSnakeReceived;
        public event SpawnFoodReceived OnSpawnFoodReceived;
        public event PauseReceived OnPauseReceived;
        public object Count => _packets.Count;


        public void AddPacket<T>(Protocol protocol, T packet) where T : IPacket
        {
            _packets.Add(new KeyValuePair<Protocol,IPacket>(protocol, packet));
        }

        private void ProcessNext()
        {
            var pair = _packets[_counter];

            switch (pair.Key)
            {
                case Protocol.Start:
                    OnStartGameReceived?.Invoke((StartGamePacket) pair.Value);
                    break;
                case Protocol.Disconnect:
                    OnDisconnectGameReceived?.Invoke((DisconnectGamePacket) pair.Value);
                    break;
                case Protocol.MoveSnake:
                    OnMoveSnakeReceived?.Invoke((MoveSnakePacket) pair.Value);
                    break;
                case Protocol.SpawnFood:
                    OnSpawnFoodReceived?.Invoke((SpawnFoodPacket) pair.Value);
                    break;
                case Protocol.Pause:
                    OnPauseReceived?.Invoke((PauseGamePacket) pair.Value);
                    break;
                case Protocol.None:
                    return;
                default:
                    return;
            }

            _counter++;
        }

        public void ProcessAll()
        {
            for (var i = 0; i < _packets.Count; i++)
            {
                ProcessNext();
            }

            Clear();
        }

        public bool Contains(Protocol protocol) => _packets.Count(x => x.Key == protocol) != 0;
        
        public void Clear()
        {
            _counter = 0;
            _packets.Clear();
        }
        
        public void Assimilate(GamePacket packet)
        {
            for (int i = 0; i < packet._packets.Count; i++)
            {
                _packets.Add(packet._packets[i]);
            }
        }
        
        public bool IsEmpty() => _packets.Count == 0;
        
        public byte[] ToBytes()
        {
            using var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(memoryStream, _packets);

            var compressed = Compress(memoryStream.ToArray());
            return compressed;
        }

        public void FromBytes(byte[] arrBytes)
        {
            using var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();               
            var decompressed = Decompress(arrBytes);
 
            memoryStream.Write(decompressed, 0, decompressed.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            
            this._packets = (List<KeyValuePair<Protocol,IPacket>>) binaryFormatter.Deserialize(memoryStream);
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
    
    
    public interface IPacket
    {
        public int ClientId {get; set;}
    }
    
    [Serializable]
    public class StartGamePacket: IPacket
    {
        public int ClientId {get; set;}
        public int GeneratedId;
        public Dictionary<int, NetworkVector> SnakePositions;
        public float TargetFrameRate;
        public bool Classic = false;

        public StartGamePacket()
        {
            ClientId = -1;
            SnakePositions = new Dictionary<int, NetworkVector>();
        }
    }

    [Serializable]
    public class DisconnectGamePacket : IPacket
    {
        public int ClientId { get; set; }
    }


    [Serializable]
    public class MoveSnakePacket : IPacket
    {
        public int ClientId { get; set; }
        public NetworkVector SnakeMarkerVector;
        public float SyncDelta;
    }

    [Serializable]
    public class SpawnFoodPacket : IPacket
    {
        public int ClientId { get; set; }
        public NetworkVector NextFoodPosition;
    }

    [Serializable]
    public class PauseGamePacket : IPacket
    {
        public int ClientId { get; set; }
        public bool PauseState;
    }
    
    
    [Serializable]
    public class NetworkVector
    {
        public float X, Y;

        public NetworkVector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public NetworkVector(Vector2 vec)
        {
            X = vec.X;
            Y = vec.Y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }

    public static class MethodExtensions
    {
        public static NetworkVector ToNetworkVector(this Vector2 vec) => new NetworkVector(vec);
    }
    
}
