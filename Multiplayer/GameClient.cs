using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace YetAnotherSnake.Multiplayer
{
    public class GameClient : IDisposable
    {
        private TcpClient _clientSocket;
        private NetworkStream _serverStream;
        private Thread _thread;
        private byte _id;

        public bool Connected = false, GameStarted;

        private VirtualButton _rightArrow;
        private VirtualButton _leftArrow;
        private Task<int> _reading; 
        
        public delegate void OnClient();

        public event OnClient onClient;


        public GameClient()
        {
            _id = (byte) Nez.Random.Range(100, 255);
            
            _leftArrow = new VirtualButton();
            _leftArrow.AddKeyboardKey(Keys.Left);
            _leftArrow.AddKeyboardKey(Keys.A);

            _rightArrow = new VirtualButton();
            _rightArrow.AddKeyboardKey(Keys.Right);
            _rightArrow.AddKeyboardKey(Keys.D);
            
            _thread = new Thread(Process);
            _thread.Start();
        }

        private void Process()
        {
            while (true)
            {
                if (!Connected) continue;
                if (GameStarted)
                {
                    if (_leftArrow.IsDown)
                    {
                        SendData(new GamePacket()
                        {
                            Disconnect = false,
                            StartGame = GameStarted,
                            LeftKeyDown = true
                        });
                    }

                    if (_leftArrow.IsReleased)
                    {
                        SendData(new GamePacket()
                        {
                            Disconnect = false,
                            StartGame = GameStarted,
                            LeftKeyDown = false
                        });
                    }

                    if (_rightArrow.IsDown)
                        SendData(new GamePacket()
                        {
                            Disconnect = false,
                            StartGame = GameStarted,
                            RightKeyDown = true
                        });

                    if (_rightArrow.IsReleased)
                    {
                        SendData(new GamePacket()
                        {
                            Disconnect = false,
                            StartGame = GameStarted,
                            RightKeyDown = false
                        });
                    }
                }

                if (!_serverStream.DataAvailable) continue;
                
                var inStream = new byte[10025];
                _reading = _serverStream.ReadAsync(inStream, 0, inStream.Length);
                var data = GamePacket.FromBytes(inStream);

                if (data.Id != 0)
                    _id = data.Id;

                //Console.WriteLine("sd");
                //Console.WriteLine(data.StartGame);

                if (data.StartGame)
                    GameStarted = true;

                
            }
        }


        public void InitClient(string address, int port)
        {
            _clientSocket = new TcpClient();
            _clientSocket.Connect(address, port);
            _serverStream = _clientSocket.GetStream();
            Connected = true;

           

            onClient?.Invoke();
        }

        public void Disconnect()
        {
            _reading.Wait();
            SendData(new GamePacket()
            {
                Disconnect = true
            });
            Dispose();
            onClient?.Invoke();
        }

        public void SendData(GamePacket data)
        {
            _serverStream = _clientSocket.GetStream();
            var outStream = GamePacket.GetBytes(data);
            _serverStream.Write(outStream, 0, outStream.Length);
            _serverStream.Flush();
        }


        public void Dispose()
        {
            Connected = false;
            _clientSocket?.Dispose();
            _serverStream?.Dispose();
        }
    }
}