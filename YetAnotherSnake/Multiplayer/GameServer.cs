using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using YetAnotherSnake.Components;
using YetAnotherSnake.Scenes;
using Random = Nez.Random;
using Timer = System.Timers.Timer;

namespace YetAnotherSnake.Multiplayer
{
    public class GameServer : IDisposable
    {
        private TcpListener _serverSocket;
        private TcpClient _clientSocket = default(TcpClient);
        private List<HandleClient> _handlers;
        
        private string _address = "0.0.0.0";
        private int _connectionCount = 0;

        private Thread _serverThread;
        private bool _serverOn = true;
        
        public bool isWorking = false;

        public ServerIDManager idManager;
        public List<HandleClient> Clients => _handlers;

        public string Address => _address;
        public int ConnectedCount => _connectionCount;

        public delegate void OnConnected();

        public event OnConnected ConnectEvent;

        public GameServer()
        {
            idManager = new ServerIDManager();

            _address = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)
                ?.ToString();

            _serverSocket = new TcpListener(IPAddress.Any, 8888);
            _serverSocket.Start();
            Console.WriteLine("Server start listening");

            _handlers = new List<HandleClient>();
            _serverThread = new Thread(ServerUpdate);
            _serverThread.Start();
        }

        private void ServerUpdate()
        {
            while (true)
            {
                if (!_serverOn) return;
                _clientSocket = _serverSocket.AcceptTcpClient();
                _handlers.Add(new HandleClient(
                    _clientSocket, idManager.GenerateNext()));
                _connectionCount++;
                ConnectEvent?.Invoke();
                Console.WriteLine($"Connected client: {_connectionCount}");
            }
        }


        public void Dispose()
        {
            _serverThread.Interrupt();
            _clientSocket?.Dispose();
            _handlers.ForEach(x => x.Dispose());
            _handlers.Clear();
        }

        public void Disconnect(int id)
        {
            _connectionCount--;
            var h = _handlers.FirstOrDefault(x => x.Id == id);
            h?.Dispose();
            _handlers.Remove(h);
            ConnectEvent?.Invoke();
        }

        public void StartGame()
        {
            isWorking = true;

           

            var snakes = new Dictionary<int, NetworkVector>();

            for (var i = 0; i < _connectionCount; i++)
            {
                snakes.Add(_handlers[i].Id, MyGame.CreateRandomPositionInWindowSpace().ToNetworkVector());
            }
            
            for (var i = 0; i < _handlers.Count; i++)
            {
                Console.WriteLine($"{_handlers[i].Id} start");
                var packet = new GamePacket();
                packet.AddPacket(Protocol.Start, new StartGamePacket()
                {
                    GeneratedId = _handlers[i].Id,
                    SnakePositions = snakes
                });
                
                _handlers[i].SendDataToClient(packet);
            }
        }

        public void SyncData(GamePacket packet)
        {
            for (var i = 0; i < _handlers.Count; i++)
            {
                _handlers[i].SendDataToClient(packet);
            }
        }
        
    }

    public class ServerIDManager
    {
        private int _number = 0;
        public int GenerateNext() => ++_number;
    }


    public class HandleClient : IDisposable
    {
        private TcpClient _clientSocket;
        private NetworkStream _networkStream;
        
        public int Id = 0;
        
        private GamePacket _writePacket;
        private Timer _sendTimer;
        private Thread _readTask;
        private byte[] _bytesFrom = new byte[10025];
        
        public HandleClient(TcpClient client, int id)
        {
            _clientSocket = client;
            _networkStream = _clientSocket.GetStream();
            
            _writePacket = new GamePacket();
            _readTask = new Thread(ReceiveDataFromClient);
            _readTask.Start();
            _sendTimer = new Timer()
            {
                AutoReset = true,
                Interval = 50
            };
            _sendTimer.Elapsed += (s, e) => SendDataToClient();
            _sendTimer.Start();
            
            this.Id = id;
        }

        private void ReceiveDataFromClient()
        {
            //var i = 0;
            while (true)
            {
                //Console.WriteLine(++i);
                if (!_networkStream.DataAvailable) continue;

                _networkStream.Read(_bytesFrom, 0, _bytesFrom.Length);
                var readPacket = new GamePacket();
                readPacket.FromBytes(_bytesFrom);
                if (readPacket.Contains(Protocol.Disconnect))
                {
                    DisconnectClient();
                    return;
                }else if (readPacket.Contains(Protocol.Start)) continue;

                MyGame.GameInstance.GameServer.SyncData(readPacket);
            }
        }

        

        private void SendDataToClient()
        {
            if (_writePacket.IsEmpty()) return;
            
            _networkStream = _clientSocket.GetStream();

            var sendBytes = _writePacket.ToBytes();
            _networkStream.Write(sendBytes, 0, sendBytes.Length);
            _networkStream.Flush();
            _writePacket. Clear();
        }
        
        private void DisconnectClient()
        {
            
            MyGame.GameInstance.GameServer.Disconnect(Id);
            Dispose();
        }



        public void SendDataToClient(GamePacket packet)
        {
            _writePacket.Assimilate(packet);
        }
        
        
        
        public void Dispose()
        {
            //_working = false;
            //_thread.Interrupt();
            _sendTimer.Dispose();
            //_readTask.Dispose();
            _clientSocket?.Dispose();
        }
    }
}