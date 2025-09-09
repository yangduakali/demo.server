using network.server.message;
using System.Threading.Tasks;

namespace network.server {
    public interface INetworkEntity : IMessageSerializable {
        ushort ConnectionId { get; }
        ushort Id { get; }
        INetworkGroup NetworkGroup { get; }

        Task Initialize(ushort connectionId, string identifier);
        Task EnterGroupAsync(INetworkGroup group);
        Task ExitGroupAsync(INetworkGroup group);
        void ReciveInputMessage(IMessage message);
    }
}
