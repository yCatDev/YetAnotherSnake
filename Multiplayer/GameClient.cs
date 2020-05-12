using System;
using System.Net.Sockets;
using System.Threading;

namespace YetAnotherSnake.Multiplayer
{
    public class GameClient : IDisposable
    {
        private TcpClient _clientSocket;
        private NetworkStream _serverStream;
        private Thread _thread;
        private byte _id;
        
        public bool Connected = false;

        public delegate void OnClient();
        public event OnClient onClient;
        

        public GameClient()
        {
            _id = (byte) Nez.Random.Range(100, 255);
            _thread = new Thread(Process);
            _thread.Start();
        }

        private void Process()
        {
            while (Connected)
            {
                var inStream = new byte[10025];
                _serverStream.Read(inStream, 0, inStream.Length);
                var data = System.Text.Encoding.ASCII.GetString(inStream);
                
                if (GamePacket.IsIdPacket(data))
                    _id = GamePacket.GetIdFromData(data);
                
                if (!string.IsNullOrEmpty(data))
                    Console.WriteLine($"Client: {data}");
            }
        }

        public void InitClient(string address, int port)
        {
            _clientSocket = new TcpClient();
            _clientSocket.Connect(address, port);
            Connected = true;
            
            onClient?.Invoke();
        }

        public void Disconnect()
        {
            SendData($"discon!");
            Dispose();
            onClient?.Invoke();
        }

        public void SendData(string data)
        {
            _serverStream = _clientSocket.GetStream();
            var outStream = System.Text.Encoding.ASCII.GetBytes($"{data}");
            _serverStream.Write(outStream, 0, outStream.Length);
            _serverStream.Flush();
        }


        public void Dispose()
        {
            Connected = false;
            _thread.Interrupt();
            _clientSocket?.Dispose();
            _serverStream?.Dispose();
        }
    }
}