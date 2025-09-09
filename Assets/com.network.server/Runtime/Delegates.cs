using network.server.message;

namespace network.server {
    public delegate void ServerMessageSender(byte[] message, ushort clientId, SendMode sendMode = SendMode.Reliable);
    public delegate void ServerMessageSendAll(byte[] message, SendMode sendMode = SendMode.Reliable);
    public delegate void ServerMessageReciver(ushort connectionId, byte[] message);

    public delegate bool ValidateNetworkGroup(string name, ushort id, out INetworkGroup networkGroup);

}
