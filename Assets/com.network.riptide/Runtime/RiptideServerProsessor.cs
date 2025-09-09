using Riptide;
using System;
using Server = Riptide.Server;
using Message = Riptide.Message;
using network.server.message;

#pragma warning disable IDE0051 // Remove unused private members
namespace network.server.riptide
{
    public class RiptideServerProsessor : IServerProsessor
    {
        public RiptideServerProsessor() {
            _server = new Server() {
                HandleConnection = ConnectionHandle
            };
            _server.ClientDisconnected += ClientDisconnected;
            _instance = this;
        }
        private static RiptideServerProsessor _instance;

        public bool IsRunning => _server.IsRunning;

        public event Action<ushort, string> OnClientConnect = delegate{ };
        public event Action<ushort> OnClientDisconnect = delegate{ };
        public event ServerMessageReciver OnEnterGroup = delegate{ };
        public event ServerMessageReciver OnExitGroup = delegate{ };
        public event ServerMessageReciver OnCustomMessage = delegate{ };
        public event ServerMessageReciver OnInputMessage = delegate{ };

        private readonly Server _server;

        public void Start(ushort port, ushort maxClient) {
            _server.Start(port, maxClient);
        }
        public void Stop() {
            _server.Stop();
        }
        public void Tick() {
            _server.Update();
        }
        public void SendWelcomeMessage(IMessage message, ushort connectionId) {
            _server.Send(GetMessage(1, message), connectionId);
        }

        public void SendEnterGroupMessage(IMessage message, ushort connectionId) {
            _server.Send(GetMessage(2, message), connectionId);
        }
        public void SendClientEnterGroupMessage(IMessage message, ushort connectionId) {
            _server.Send(GetMessage(3, message), connectionId);
        }
        public void SendClientExitGroupMessage(IMessage message, ushort connectionId) {
            _server.Send(GetMessage(4, message), connectionId);
        }
        public void SendCustomMessage(IMessage message, ushort connectionId) {
            _server.Send(GetMessage(5, message), connectionId);
        }
        public void SendCustomMessageToAll(IMessage message) {
            _server.SendToAll(GetMessage(5, message));
        }
        public void SendNetworkComponentMessageToAll(IMessage message) {
            _server.SendToAll(GetMessage(6, message));
        }
        public void SendExitGroupMessage(IMessage message, ushort connectionId) {
            _server.Send(GetMessage(7, message), connectionId);
        }

        private void ConnectionHandle(Connection pendingConnection, Message connectMessage) {
            _server.Accept(pendingConnection);
            OnClientConnect?.Invoke(pendingConnection.Id, connectMessage.GetString());
        }
        private void ClientDisconnected(object sender, ServerDisconnectedEventArgs e) {
            OnClientDisconnect?.Invoke(e.Client.Id);
        }
        private Message GetMessage(ushort id, IMessage message) {
            switch (message.SendMode) {
                case SendMode.Unreliable:
                    Message msg = Message.Create(MessageSendMode.Unreliable, id).AddBytes(message.WritedBytes.AddSendByte());
                    message.Release();
                    return msg;
                case SendMode.Reliable:
                    Message msg2 = Message.Create(MessageSendMode.Unreliable, id).AddBytes(message.WritedBytes.AddSendByte());
                    message.Release();
                    return msg2;
                default: return Message.Create();
            }
        }

        [MessageHandler(1)]
        private static void Message_EnterGroup(ushort id, Message message) {
            _instance.OnEnterGroup?.Invoke(id, message.GetBytes().AddReciveByte());
        }
        [MessageHandler(2)]
        private static void Message_ExitGroup(ushort id, Message message) {
            _instance.OnExitGroup?.Invoke(id, message.GetBytes().AddReciveByte());
        }
        [MessageHandler(3)]
        private static void Message_Custom(ushort id, Message message) {
            _instance.OnCustomMessage?.Invoke(id, message.GetBytes().AddReciveByte());
        }
        [MessageHandler(4)]
        private static void Message_Input(ushort id, Message message) {
            _instance.OnInputMessage?.Invoke(id, message.GetBytes().AddReciveByte());
        }

    }
}
