using actor;
using assetmanager;
using game.actors;
using network.server;
using network.server.component;
using network.server.message;
using System.Threading.Tasks;
using UnityEngine;

namespace mrpg.server {
    public class MRPG_NetworkEntity : NetworkEntity {

        public MRPG_Player Player { get; private set; }
        public string StringData { get; private set; }

        public override Task Initialize(ushort connectionId, string identifier) {
            ConnectionId = connectionId;
            Id = (ushort)Random.Range(1, ushort.MaxValue - 1);
            StringData = identifier;
            return Task.CompletedTask;
        }
        public override async Task EnterGroupAsync(INetworkGroup group) {
            var playerObject = await AssetManager.InstantiateAsync(StringData);
            Player = await ActorUltis.CreatePlayerAsync<MRPG_Player>(playerObject);
            Player.NetworkEntity = this;
            Player.InputReciverModule = Player.AddModule<InputReciverModule>(0);
            Player.AddModule<LocomotionModule>(1);
            Player.Initialize();
            Player.Root.AddComponent<NetworkTransform>().Initialize($"player{ConnectionId}");
            Debug.Log(Player.Animator == null);
            Player.Root.AddComponent<NetworkAnimator>().Initialize($"player{ConnectionId}", Player.Animator);
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(Player.Root, group.Scene);
            await base.EnterGroupAsync(group);
        }
        public override Task ExitGroupAsync(INetworkGroup group) {
            Player.Destroy();
            return base.ExitGroupAsync(group);
        }
        public override void ReciveInputMessage(IMessage message) {
            Player.InputReciverModule.moveInput = message.GetVector2();
            Player.InputReciverModule.lookInput = message.GetVector2();
            Player.InputReciverModule.cameraRotationn = message.GetQuaternion();
            message.Release();
        }
        public override void Serialize(IMessage message) {
            base.Serialize(message);
            message.Add(StringData);
        }
        public override void Deserialize(IMessage message) {
            base.Deserialize(message);
            StringData = message.GetString();
        }
    }
}
