using network.server.message;
using System;

namespace network.server {
    public interface IServerProsessor {
        bool IsRunning { get; }

        event Action<ushort , string> OnClientConnect;
        event Action<ushort> OnClientDisconnect;
        event ServerMessageReciver OnEnterGroup;
        event ServerMessageReciver OnExitGroup;
        event ServerMessageReciver OnCustomMessage;
        event ServerMessageReciver OnInputMessage;

        void Start(ushort port, ushort maxClient);
        void Stop();
        void Tick();

        void SendWelcomeMessage(IMessage message, ushort connectionId);
        void SendEnterGroupMessage(IMessage message, ushort connectionId);
        void SendClientEnterGroupMessage(IMessage message, ushort connectionId);
        void SendClientExitGroupMessage(IMessage message, ushort connectionId);
        void SendCustomMessage(IMessage message, ushort connectionId);
        void SendCustomMessageToAll(IMessage message);
        void SendNetworkComponentMessageToAll(IMessage message);
        void SendExitGroupMessage(IMessage message, ushort connectionId);
    }
}
