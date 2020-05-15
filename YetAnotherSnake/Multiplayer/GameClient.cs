using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Nez;
using YetAnotherSnake.Scenes;
using Random = Nez.Random;
using Timer = System.Timers.Timer;

namespace YetAnotherSnake.Multiplayer
{
    public class GameClient : IDisposable
    {
        private TcpClient _clientSocket;
        private NetworkStream _serverStream;

        public bool Connected = false, GameStarted;

        public float PossibleWidth, PossibleHeight;
        
        //private Task<int> _reading; 
        
        public delegate void ClientChanged();
        public Dictionary<int, NetworkVector> Snakes { get; private set; }
        
        
        private Timer _writeTimer;
        private Thread _readTask;
        
        public event ClientChanged OnClient;

        private GamePacket _writePacket, _readPacket;
        

        public int Id { get; private set; }

        public void InitClient(string address, int port)
        {
            _clientSocket = new TcpClient();
            _clientSocket.Connect(address, port);
            _serverStream = _clientSocket.GetStream();
            
            Connected = true;
            

            _writePacket = new GamePacket();
            _readPacket = new GamePacket();
            
            _readPacket.OnStartGameReceived += OnGameStartReceived;
            
            _writeTimer = new Timer {Interval = 50, AutoReset = true};
            _writeTimer.Elapsed += (sender, args) => SendDataToServer();
            _writeTimer.Start();

            _readTask = new Thread(ReceivePacketFromServer);
            _readTask.Start();
            
            OnClient?.Invoke();
        }

        private void ReceivePacketFromServer()
        {
            
            while (true)
            {
                if (!Connected) continue;
                if (!_serverStream.DataAvailable) continue;

                var inStream = new byte[10025];
                _serverStream.Read(inStream, 0, inStream.Length);
                _readPacket = GamePacket.FromBytes(inStream);


                _readPacket.ProcessAll();
            }
        }
        private void SendDataToServer()
        {
            //if (!GameStarted) return;
            if (!Connected) return;
            if (_writePacket.IsEmpty()) return;
            
            _serverStream = _clientSocket.GetStream();
            var outStream = _writePacket.ToBytes();
            _serverStream.Write(outStream, 0, outStream.Length);
            _serverStream.Flush();
            
            _writePacket.Clear();
        }
        
        private void OnGameStartReceived(StartGamePacket packet)
        {
            Id = packet.ClientId;
            MenuScene.Instance.RemovePostProcessor(MyGame.GameInstance.BloomPostProcessor);
            MenuScene.Instance.RemovePostProcessor(MyGame.GameInstance.VignettePostProcessor);

            Snakes = packet.SnakePositions;

            Core.StartSceneTransition(new FadeTransition(()
                => new GameScene()));
        }

      

        

        public void Disconnect()
        {
           // _reading.Wait();
           Console.WriteLine($"Disconnect {Id}");
           _writePacket.AddPacket(Protocol.Disconnect, new DisconnectGamePacket()
           {
              ClientId = Id
           });
           Thread.Sleep(100);
           Dispose();
           OnClient?.Invoke();
        }

        public void SpawnFood()
        {
            /*var possibleWidth = 1280;
            var possibleHeight = 720;
            Packet.SpawnFood = true;
            Packet.NextFoodPosition = (Random.Range(-possibleWidth, possibleWidth),
                Random.Range(-possibleHeight, possibleHeight));*/
            //SendData(packet);
        }
        
        
        


        public void Dispose()
        {
            Connected = false;
            _writeTimer.Dispose();
            _clientSocket?.Dispose();
            _serverStream?.Dispose();
        }
    }
}