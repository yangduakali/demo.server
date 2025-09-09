using network.server.message;
using network.server.ultis;
using UnityEngine;

namespace network.server.component {
    public abstract class NetworkComponent<T> : MonoBehaviour where T : Component {
        public string Identifier;
        public T component;
        public bool initializeAtStart;
        protected bool hasInitialize;
        private ushort componentId;

        private void Start() {
            if (initializeAtStart) Initialize(Identifier, component);
        }
        private void FixedUpdate() {
            if (!hasInitialize) return;
            var msg = Message.Create(SendMode.Unreliable);
            msg.Add(componentId);
            msg.Add(Identifier);
            OnSend(msg);
            IServerManager.Instance.SendNNetworkComponentMessage(msg);
        }

        public virtual void Initialize(string identifier, T component = null) {
            Identifier = identifier;
            this.component = component == null ? gameObject.GetComponent<T>() : component;
            if (string.IsNullOrWhiteSpace(identifier)) {
                this.Identifier = gameObject.name;
            }

            if (this.component == null) {
                gameObject.SetActive(false);
                return;
            }
            componentId = Helper.GetComponentId<T>();
            hasInitialize = true;
        }
        protected abstract void OnSend(IMessage message);

        public static implicit operator T(NetworkComponent<T> component) {
            return component.component;
        }
    }

}
