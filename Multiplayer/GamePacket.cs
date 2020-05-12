using System;

namespace YetAnotherSnake.Multiplayer
{
    public static class GamePacket
    {

        public static bool isDisconnected(string data) => data.Contains("discon!");
        
        public static bool IsIdPacket(string data)
            => !string.IsNullOrEmpty(data) && data.Contains("id!");
        
        public static byte GetIdFromData(string data)
        {
            if (!IsIdPacket(data))
                throw new Exception("Recived data is not id");
            return byte.Parse(data.Substring(data.IndexOf('!'), data.Length));
        }
        
        
        
    }
}