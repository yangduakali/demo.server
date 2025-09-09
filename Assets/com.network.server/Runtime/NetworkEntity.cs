using network.server.message;
using System.Threading.Tasks;

namespace network.server {
    public abstract class NetworkEntity : INetworkEntity{
        public ushort ConnectionId { get; protected set; }
        public ushort Id { get; protected set; }
        public INetworkGroup NetworkGroup { get; protected set; }
        public abstract Task Initialize(ushort connectionId, string identifier);
        public abstract void ReciveInputMessage(IMessage message);

        public virtual Task EnterGroupAsync(INetworkGroup group) {
            NetworkGroup = group;
            return group.AddClient(this);
        }
        public virtual Task ExitGroupAsync(INetworkGroup group) {
            NetworkGroup = default;
            return group.RemoveClient(this);
        }
        public virtual void Serialize(IMessage message) {
            message.Add(ConnectionId);
            message.Add(Id);
        }
        public virtual void Deserialize(IMessage message) {
            ConnectionId = message.GetUShort();
            Id = message.GetUShort();
        }

    }
}
