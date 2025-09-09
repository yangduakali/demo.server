using network.server.message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace network.server {

    public class ServerManager<T, T2, T3> : IServerManager where T : class, IServerProsessor, new() where T2 : class, INetworkEntity, new() where T3 : class, INetworkGroup, new() {
        public ServerManager() {
            _prosessor = new T();
            _prosessor.OnClientConnect += Prossesor_OnClientConnect;
            _prosessor.OnClientDisconnect += Prossesor_OnClientDisconnectAsync;
            _prosessor.OnEnterGroup += Prossesor_OnEnterGroupAsync;
            _prosessor.OnExitGroup += Prossesor_OnExitGroupAsync;
            _prosessor.OnCustomMessage += Prossesor_OnCustomMessage;
            _prosessor.OnInputMessage += Prossesor_OnInputMessage;

            IServerManager.Instance = this;
        }

        public ushort Port { get; set; } = 8080;
        public ushort MaxClient { get; set; } = 100;
        public Dictionary<ushort, INetworkEntity> ConnectedClient { get; } = new();
        public Dictionary<string, Dictionary<ushort, INetworkGroup>> NetworkGroups { get; } = new();
        public ValidateNetworkGroup ValidateNetworkGroupHandler { get; set; }
        public bool IsRunnning { get => _prosessor.IsRunning; }

        private readonly IServerProsessor _prosessor;

        public event Action<INetworkEntity> OnClientConnected;
        public event Action<ushort> OnClientDisconnected;
        public event Action<INetworkEntity, INetworkGroup> OnClientEnterGroup;
        public event Action<INetworkEntity, INetworkGroup> OnClientExitGroup;
        public event Action<INetworkEntity, IMessage> OnCustomMessage;

        public void Start() {
            _prosessor.Start(Port, MaxClient);
        }
        public void Stop() {
            _prosessor.Stop();
        }
        public void Tick() {
            _prosessor.Tick();
        }
        public void SendCustomMessageToAll(IMessage message) {
        }
        public void SendCustomMessage(IMessage message, INetworkEntity client) {
        }
        public void SendNNetworkComponentMessage(IMessage message) { 
            _prosessor.SendNetworkComponentMessageToAll(message);
        }

        private async void Prossesor_OnClientConnect(ushort connectionId, string identifier) {
            var newClient = new T2();
            await newClient.Initialize(connectionId, identifier);
            ConnectedClient.Add(connectionId, newClient);

            var msg = Message.Create();
            msg.AddClass(newClient);
            _prosessor.SendWelcomeMessage(msg, connectionId);
            OnClientConnected?.Invoke(newClient);
            msg.Release();
        }
        private async void Prossesor_OnClientDisconnectAsync(ushort connectionId) {
            var exitClient = ConnectedClient[connectionId];
            if (exitClient.NetworkGroup != null) {
                var group = exitClient.NetworkGroup;
                await ExitGroupAsync(group, exitClient);
            }
            ConnectedClient.Remove(connectionId);
            OnClientDisconnected?.Invoke(connectionId);
        }
        private async void Prossesor_OnEnterGroupAsync(ushort connectionId, byte[] message) {
            var msg = Message.Create(bytes: message);
            var groupName = msg.GetString();
            var groupId = msg.GetUShort();
            var targetClient = ConnectedClient[connectionId];

            T3 group;
            if (!ValidateNetworkGroup(groupName, groupId, out var validGroup) || validGroup == null) {
                group = null;
                ProssesMessage("NetworkGroup name not valid or group not availble on server");
                return;
            }

            if (groupId > 0) {
                if (!NetworkGroups.ContainsKey(groupName) || !NetworkGroups[groupName].ContainsKey(groupId)) {
                    group = null;
                    ProssesMessage("You try enter existing group, but not found");
                    return;
                }
                var instanceGroup  = NetworkGroups[groupName][groupId];
                await targetClient.EnterGroupAsync(instanceGroup);
                group = (T3)instanceGroup;
                ProssesMessage();
                return;
            }

            if (NetworkGroups.TryGetValue(groupName, out var groupStack)) {
                var targetGroup = groupStack.First().Value;
                if (validGroup.IsInstance || (!validGroup.IsInstance && targetGroup == null)) {
                    ushort rndId;
                    do {
                        rndId = (ushort)UnityEngine.Random.Range(1, ushort.MaxValue);
                    } while (groupStack.ContainsKey(rndId));

                    validGroup.Id = rndId;
                    await validGroup.ProssesAsync();
                    await targetClient.EnterGroupAsync(validGroup);
                    groupStack.Add(validGroup.Id, validGroup);
                    group = (T3)validGroup;
                    ProssesMessage();
                    return;
                }
                group = (T3)targetGroup;
                await targetClient.EnterGroupAsync(targetGroup);
                ProssesMessage();
                return;
            }
            validGroup.Id = (ushort)UnityEngine.Random.Range(1, ushort.MaxValue);
            NetworkGroups.Add(groupName, new());
            await validGroup.ProssesAsync();
            await targetClient.EnterGroupAsync(validGroup);
            group = (T3)validGroup;
            NetworkGroups[groupName].Add(validGroup.Id, validGroup);
            ProssesMessage();

            void ProssesMessage(string messageServer = "") {
                msg.Clear();
                if (group == null) {
                    msg.Add(false);
                    msg.Add(messageServer);
                    _prosessor.SendEnterGroupMessage(msg, targetClient.ConnectionId);
                    return;
                }

                msg.Add(true);
                msg.Add("success");
                msg.AddClass(group);

                _prosessor.SendEnterGroupMessage(msg, targetClient.ConnectionId);
                BroadcastEnterGroup(group, targetClient);
                OnClientEnterGroup?.Invoke(targetClient, group);
                msg.Release();
            }
        }
        private async void Prossesor_OnExitGroupAsync(ushort connectionId, byte[] message) {
            var msg = Message.Create(bytes:message);
            var targetClient = ConnectedClient[connectionId];

            if(targetClient.NetworkGroup == null) {
                msg.Add(false);
                msg.Add("Client is not on group");
                _prosessor.SendExitGroupMessage(msg, targetClient.ConnectionId);
                return;
            }

            await ExitGroupAsync(targetClient.NetworkGroup, targetClient);
            msg.Add(true);
            msg.Add("Succes");
            _prosessor.SendExitGroupMessage(msg, targetClient.ConnectionId);

        }
        private void Prossesor_OnCustomMessage(ushort connectionId, byte[] message) {
        }
        private void Prossesor_OnInputMessage(ushort connectionId, byte[] message) {
            ConnectedClient[connectionId].ReciveInputMessage(Message.Create(bytes: message));
        }
        private void BroadcastEnterGroup(INetworkGroup group, INetworkEntity client) {
            var broadMsg = Message.Create();
            foreach (var item in group.Clients) {
                if (item.Value.ConnectionId == client.ConnectionId) continue;
                broadMsg.AddClass((T2)client);
                _prosessor.SendClientEnterGroupMessage(broadMsg, item.Value.ConnectionId);
            }
            broadMsg.Release();
        }
        private void BroadcastExitGroup(INetworkGroup group, INetworkEntity entity) {
            var broadMsg = Message.Create();
            foreach (var item in group.Clients) {
                if (item.Value.ConnectionId == entity.ConnectionId) continue;
                broadMsg.Add(entity.ConnectionId);
                _prosessor.SendClientExitGroupMessage(broadMsg, item.Value.ConnectionId);
            }
            broadMsg.Release();
        }
        private async Task ExitGroupAsync(INetworkGroup group, INetworkEntity client) {
            var msg = new Message();
            await client.ExitGroupAsync(group);
            OnClientExitGroup?.Invoke(client, group);
            BroadcastExitGroup(group, client);
            await CleaningGroupAsync(group);
        }
        private Task CleaningGroupAsync(INetworkGroup group) {
            if (group.Clients.Count > 0) return Task.CompletedTask;
            NetworkGroups[group.Name].Remove(group.Id);
            if (NetworkGroups[group.Name].Count == 0) NetworkGroups.Remove(group.Name);
            return group.RealeaseAsync();
        }
        protected virtual bool ValidateNetworkGroup(string name, ushort id, out INetworkGroup networkGroup) {
            if (ValidateNetworkGroupHandler != null) {
                return ValidateNetworkGroupHandler(name, id, out networkGroup);
            }
            networkGroup = null;
            return false;
        }

    }


    
}
