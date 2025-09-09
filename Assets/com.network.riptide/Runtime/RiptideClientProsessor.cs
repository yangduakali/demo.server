//using network.message;
//using Riptide;
//using System;
//using Message = Riptide.Message;

//#pragma warning disable IDE0051 // Remove unused private members
//namespace network.client.riptide {
//    public class RiptideClientProsessor : IClientProsessor {
//        public RiptideClientProsessor() {
//            _instance = this;
//            _client = new Client();
//            _client.ConnectionFailed += ConnectionFailed;

//        }
//        private static RiptideClientProsessor _instance;

//        public event Action OnConnectFailed = delegate{ };
//        public event ClientMessageReciver OnWelocomeMessage;
//        public event ClientMessageReciver OnEnterGroupMessage;
//        public event ClientMessageReciver OnClientEnterGroupMessage;
//        public event ClientMessageReciver OnClientExitGroupMessage;
//        public event ClientMessageReciver OnCustomMessage;
//        public event ClientMessageReciver OnNetworkComponentMessage;

//        private readonly Client _client;

//        public void Connect(string ip, ushort port, string identifier = "") {
//            var msg = Message.Create();
//            msg.Add(identifier);
//            _client.Connect($"{ip}:{port}", message: msg);
//        }
//        public void Disconnect() {
//            _client.Disconnect();
//        }
//        public void Tick() {
//            _client.Update();
//        }
//        public void SendMessageEnterGroup(IMessage message) {
//            _client.Send(GetMessage(1, message));
//        }
//        public void SendMessageExitGroup(IMessage message) {
//            _client.Send(GetMessage(2, message));
//        }
//        public void SendCustomMessage(IMessage message) {
//            _client.Send(GetMessage(3, message));
//        }
//        public void SendInputMessage(IMessage message) {
//            _client.Send(GetMessage(4, message));
//        }

//        private void ConnectionFailed(object sender, ConnectionFailedEventArgs e) {
//            OnConnectFailed?.Invoke();
//        }
//        private Message GetMessage(ushort id, IMessage message) {
//            switch (message.SendMode) {
//                case SendMode.Unreliable:
//                    Message msg1 = Message.Create(MessageSendMode.Unreliable, id).AddBytes(message.WritedBytes);
//                    message.Release();
//                    return msg1;
//                case SendMode.Reliable:
//                    Message msg2 = Message.Create(MessageSendMode.Unreliable, id).AddBytes(message.WritedBytes);
//                    message.Release();
//                    return msg2;
//                default: return Message.Create();
//            }
//        }

//        [MessageHandler(1)]
//        private static void Message_Welcome(Message message) {
//            _instance.OnWelocomeMessage?.Invoke(message.GetBytes());
//        }
//        [MessageHandler(2)]
//        private static void Message_EnterGroup(Message message) {
//            _instance.OnEnterGroupMessage?.Invoke(message.GetBytes());
//        }
//        [MessageHandler(3)]
//        private static void Message_ClientEnterGroup(Message message) {
//            _instance.OnClientEnterGroupMessage?.Invoke(message.GetBytes());
//        }
//        [MessageHandler(4)]
//        private static void Message_ClientExitGroup(Message message) {
//            _instance.OnClientExitGroupMessage?.Invoke(message.GetBytes());
//        }
//        [MessageHandler(5)]
//        private static void Message_Custom(Message message) {
//            _instance.OnCustomMessage?.Invoke(message.GetBytes());
//        }
//        [MessageHandler(6)]
//        private static void Message_NetworkComponent(Message message) {
//            _instance.OnNetworkComponentMessage?.Invoke(message.GetBytes());
//        }

//    }
//}
