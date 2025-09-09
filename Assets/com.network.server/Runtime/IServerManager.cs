using network.server.message;
using System.Collections.Generic;
using System;

namespace network.server {
    public interface IServerManager {
        ushort Port { get; set; }
        ushort MaxClient { get; set; }
        Dictionary<ushort, INetworkEntity> ConnectedClient { get; }
        Dictionary<string, Dictionary<ushort, INetworkGroup>> NetworkGroups { get; }
        ValidateNetworkGroup ValidateNetworkGroupHandler { get; set; }
        bool IsRunnning { get; }

        event Action<INetworkEntity> OnClientConnected;
        event Action<ushort> OnClientDisconnected;
        event Action<INetworkEntity, INetworkGroup> OnClientEnterGroup;
        event Action<INetworkEntity, INetworkGroup> OnClientExitGroup;
        event Action<INetworkEntity, IMessage> OnCustomMessage;

        void Start();
        void Stop();
        void Tick();
        void SendCustomMessageToAll(IMessage message);
        void SendCustomMessage(IMessage message, INetworkEntity client);
        void SendNNetworkComponentMessage(IMessage message);

        static IServerManager Instance { get; internal set; }
    }
}
