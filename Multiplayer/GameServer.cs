using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YetAnotherSnake.Components;
using YetAnotherSnake.Scenes;

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
            _handlers.Remove(_handlers.FirstOrDefault(x => x.id == id));
            ConnectEvent?.Invoke();
        }

        public void StartGame()
        {
            isWorking = true;
            var ids = new int[_connectionCount];
            for (var j = 0; j < _connectionCount; j++)
                ids[j] = _handlers[j].id;
            
            for (var i = 0; i < _handlers.Count; i++)
            {
                Console.WriteLine($"{_handlers[i].id} start");
               
                _handlers[i].SendDataToClient(new GamePacket()
                {
                    ServiceData = true,
                    idsToCreate = ids,
                    Id = _handlers[i].id,
                    StartGame = true
                });
            }
        }

        public void SyncData(int id, GamePacket packet)
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
        private Task _thread;
        private NetworkStream _networkStream;
        private byte[] _bytesFrom = new byte[10025];
        public int id = 0;
        private bool _working = true;

        public HandleClient(TcpClient client, int id)
        {
            _clientSocket = client;
            _thread = Task.Run(Process);
            SendDataToClient(new GamePacket()
            {
                Id = id
            });
            this.id = id;
        }

        private void Process()
        {
            while (_working)
            {
                var data = GetDataFromClient();
                    if (data == null) continue;
                    if (data.Disconnect && !data.StartGame)
                    {
                        Console.WriteLine($"Disconnecting {id}");
                        DisconnectClient();
                    }

                    if (data.Test)
                        Console.WriteLine("Test");
                    
                    Console.WriteLine("Got package");
                    if (!data.ServiceData)
                        MyGame.GameInstance.GameServer.SyncData(id, data);
            }
        }

        private void DisconnectClient()
        {
            MyGame.GameInstance.GameServer.Disconnect(id);
            Dispose();
        }

        private GamePacket GetDataFromClient()
        {
            if (_networkStream.DataAvailable) return null;
            
            _networkStream.Read(_bytesFrom, 0, _bytesFrom.Length);
            return GamePacket.FromBytes(_bytesFrom);
        }

        public void SendDataToClient(GamePacket packet)
        {
            _networkStream = _clientSocket.GetStream();
            var sendBytes = GamePacket.GetBytes(packet);
            _networkStream.Write(sendBytes, 0, sendBytes.Length);
            _networkStream.Flush();
        }
        
        
        public void Dispose()
        {
            _working = false;
            //_thread.Interrupt();
            _clientSocket?.Dispose();
        }
    }
}