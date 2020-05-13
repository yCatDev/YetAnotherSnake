using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Nez;
using YetAnotherSnake.Scenes;

namespace YetAnotherSnake.Multiplayer
{
    public class GameClient : IDisposable
    {
        private TcpClient _clientSocket;
        private NetworkStream _serverStream;
        private Thread _thread;
        private int _id;

        public bool Connected = false, GameStarted;

        private VirtualButton _rightArrow;
        private VirtualButton _leftArrow;
        private bool _leftArrowWasPressed = false, _rightArrowWasPressed = false;
        private Task<int> _reading; 
        
        public delegate void OnClient();

        public event OnClient onClient;
        public int[] SnakeIds;

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
            
        }

        public int Id => _id;

        private void Process()
        {
            while (true)
            {
                if (!Connected) continue;
                if (GameStarted)
                {
                    
                    if (_leftArrow.IsDown)
                    {
                        _leftArrowWasPressed = true;
                        SendData(new GamePacket()
                        {
                            ServiceData = false,
                            LeftKeyDown = true
                        });
                    }
                    
                    if (_leftArrow.IsReleased && _leftArrowWasPressed)
                    {
                        _leftArrowWasPressed = false;
                        SendData(new GamePacket()
                        {
                            ServiceData = false,
                            LeftKeyDown = false
                        });
                    }

                    if (_rightArrow.IsDown)
                        SendData(new GamePacket()
                        {
                            RightKeyDown = true
                        });

                    if (_rightArrow.IsReleased)
                    {
                        SendData(new GamePacket()
                        {
                            RightKeyDown = false
                        });
                    }
                }

                if (!_serverStream.DataAvailable) continue;
                
                var inStream = new byte[10025];
                _reading = _serverStream.ReadAsync(inStream, 0, inStream.Length);
                var data = GamePacket.FromBytes(inStream);

                if (data.ServiceData && data.StartGame)
                {
                    GameStarted = true;
                    _id = data.Id;
                    MenuScene.Instance.RemovePostProcessor(MyGame.GameInstance.BloomPostProcessor);
                    MenuScene.Instance.RemovePostProcessor(MyGame.GameInstance.VignettePostProcessor);
                    Core.StartSceneTransition(new FadeTransition(() 
                        => new GameScene()));
                    SnakeIds = data.idsToCreate;
                }
                
                if (!data.ServiceData && GameScene.Instance!=null)
                    GameScene.Instance.ProcessData(_id, data);

            }
        }


        public void InitClient(string address, int port)
        {
            _clientSocket = new TcpClient();
            _clientSocket.Connect(address, port);
            _serverStream = _clientSocket.GetStream();
            Connected = true;
            _thread.Start();
            onClient?.Invoke();
        }

        public void Disconnect()
        {
            _reading.Wait();
            SendData(new GamePacket()
            {
                ServiceData = true,
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