using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using VoxelForge.Shared.Networking;
using VoxelForge.Shared.Networking.NetworkBridge;
using VoxelForge.Shared.Networking.Packets;
using VoxelForge.Shared.Registry;

namespace VoxelForge.Server;

/// <summary>
/// Multi-client server that handles multiple network connections.
/// </summary>
public class MultiClientServer
{
    private readonly Server _server;
    private readonly ConcurrentDictionary<int, ClientConnection> _clients = new();
    private int _nextClientId = 0;
    private TcpListener? _listener;
    private bool _isRunning = false;
    
    private class ClientConnection
    {
        public int Id { get; set; }
        public TcpClient TcpClient { get; set; }
        public NetworkBridgeNet Bridge { get; set; }
        
        public ClientConnection(int id, TcpClient tcpClient, NetworkBridgeNet bridge)
        {
            Id = id;
            TcpClient = tcpClient;
            Bridge = bridge;
        }
    }
    
    // /// Creates a multi-client server with an embedded Server instance.
    public MultiClientServer(int seed = 12345)
    {
        // Create a broadcast bridge that sends to all clients
        var broadcastBridge = new BroadcastNetworkBridge(this);
        _server = new Server(broadcastBridge, seed);
    }
    
    // /// Starts listening for client connections on the specified port.
    public async Task StartAsync(int port = 25565)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        _isRunning = true;
        
        Console.WriteLine($"Multi-client server listening on port {port}...");
        
        // Accept clients in background
        _ = Task.Run(AcceptClientsAsync);
        
        // Run server loop
        await _server.RunAsync();
    }
    
    // /// Accepts incoming client connections.
    private async Task AcceptClientsAsync()
    {
        while (_isRunning && _listener != null)
        {
            try
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                int clientId = _nextClientId++;
                
                Console.WriteLine($"Client #{clientId} connected from {tcpClient.Client.RemoteEndPoint}");
                
                var stream = tcpClient.GetStream();
                var bridge = new NetworkBridgeNet(stream, PacketRegistry.Factories);
                
                var connection = new ClientConnection(clientId, tcpClient, bridge);
                _clients.TryAdd(clientId, connection);
                
                // Handle client in background
                _ = Task.Run(() => HandleClient(connection));
            }
            catch (Exception ex)
            {
                if (_isRunning)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }
            }
        }
    }
    
    // /// Handles communication with a specific client.
    private async Task HandleClient(ClientConnection connection)
    {
        try
        {
            // Send initial registry and chunks
            var registryMappings = BlockStateRegistry.ExportMappings();
            connection.Bridge.Send(new BlockStateRegistryPacket(registryMappings));
            
            Console.WriteLine($"Sent registry to client #{connection.Id}");
            
            // Client will request chunks, server will respond via the broadcast bridge
            
            while (connection.TcpClient.Connected)
            {
                connection.Bridge.Poll();
                await Task.Delay(10);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client #{connection.Id} error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"Client #{connection.Id} disconnected");
            _clients.TryRemove(connection.Id, out _);
            connection.TcpClient.Close();
        }
    }
    
    // /// Broadcasts a packet to all connected clients.
    public void BroadcastPacket(Packet packet)
    {
        foreach (var client in _clients.Values)
        {
            try
            {
                client.Bridge.Send(packet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending to client #{client.Id}: {ex.Message}");
            }
        }
    }
    
    // /// Stops the server and disconnects all clients.
    public void Stop()
    {
        _isRunning = false;
        _listener?.Stop();
        
        foreach (var client in _clients.Values)
        {
            client.TcpClient.Close();
        }
        
        _clients.Clear();
        Console.WriteLine("Server stopped");
    }
    
    // /// Network bridge that broadcasts packets to all connected clients.
    private class BroadcastNetworkBridge : INetworkBridge
    {
        private readonly MultiClientServer _server;
        private readonly Dictionary<Type, Delegate> _handlers = new();
        
        public BroadcastNetworkBridge(MultiClientServer server)
        {
            _server = server;
        }
        
        public void Send(Packet packet)
        {
            _server.BroadcastPacket(packet);
        }
        
        public void RegisterHandler<T>(Action<T> handler) where T : Packet
        {
            _handlers[typeof(T)] = handler;
            
            // Register this handler on all client bridges
            foreach (var client in _server._clients.Values)
            {
                client.Bridge.RegisterHandler(handler);
            }
        }
        
        public void Poll()
        {
            // Polling is handled per-client in HandleClient
        }
    }
}
