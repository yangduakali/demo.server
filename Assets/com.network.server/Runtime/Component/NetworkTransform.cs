using network.server.message;
using UnityEngine;

namespace network.server.component {
    [AddComponentMenu("Com/Network/Server/Server Transform")]
    public class NetworkTransform : NetworkComponent<Transform> {

        protected override void OnSend(IMessage message) {
            message.Add(component.position);
            message.Add(component.rotation);
            message.Add(component.localScale);
        }
    }
}
