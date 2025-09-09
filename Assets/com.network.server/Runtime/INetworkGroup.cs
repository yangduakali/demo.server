using network.server.message;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;

namespace network.server {
    public interface INetworkGroup : IMessageSerializable {
        Scene Scene { get; }
        bool IsInstance { get; }
        ushort Id { get; set; }
        string Name { get; set; }
        Dictionary<ushort, INetworkEntity> Clients { get; }

        Task ProssesAsync();
        Task AddClient(INetworkEntity networkEntity);
        Task RemoveClient(INetworkEntity networkEntity);
        Task RealeaseAsync();
    }
}
