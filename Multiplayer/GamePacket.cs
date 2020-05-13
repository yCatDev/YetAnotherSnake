using System;
using System.Runtime.InteropServices;

namespace YetAnotherSnake.Multiplayer
{
    public struct GamePacket
    {
        public byte Id;
        
        public bool Disconnect;
        public bool StartGame;
        //public bool Died;
        
        public bool LeftKeyDown;
        public bool RightKeyDown;

        public bool Pause;

        //public (int, int) FoodSpawnPos;
        
        public static byte[] GetBytes(GamePacket packet) {
            var size = Marshal.SizeOf(packet);
            var arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(packet, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            
            return arr;
        }

        public static GamePacket FromBytes(byte[] data)
        {
            var str = new GamePacket();

            var size = Marshal.SizeOf(str);
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(data, 0, ptr, size);

            str = (GamePacket)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }
        
        
    }
}