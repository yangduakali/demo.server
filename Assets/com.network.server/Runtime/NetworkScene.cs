using network.server.message;
using scenemanager;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace network.server {
    public class NetworkScene<T> : INetworkGroup  where T : class , INetworkEntity,new(){
        public string Name { get; set; }
        public ushort Id { get; set; }
        public bool IsInstance { get; set; }

        public Dictionary<ushort, INetworkEntity> Clients { get; protected set; } = new(); 
        public Scene Scene { get; protected set; }

        public async Task ProssesAsync() {
            await scenemanager.SceneManager.Load(Name).RunAsync();
            var oriScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(Name);
            var copyScene = UnityEngine.SceneManagement.SceneManager.CreateScene($"{Name}-{Id}",new CreateSceneParameters{localPhysicsMode = LocalPhysicsMode.Physics3D});
            var oriRootCount = oriScene.rootCount;
            var roots = oriScene.GetRootGameObjects();
            for (int i = 0; i < oriRootCount; i++) {
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(roots[i], copyScene);
            }
            Scene = copyScene;
            await scenemanager.SceneManager.Unload(Name).RunAsync();
        }
        public Task AddClient(INetworkEntity networkEntity) {
            Clients.Add(networkEntity.ConnectionId, networkEntity);
            return Task.CompletedTask;
        }
        public Task RemoveClient(INetworkEntity networkEntity) {
            Clients.Remove(networkEntity.ConnectionId);
            return Task.CompletedTask;
        }
        public async Task RealeaseAsync() {
            var complite = false;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(Scene).completed += opr => { 
                complite = true;
            };
            while (!complite) {
                await Task.Yield();
            }
        }
        public virtual void Serialize(IMessage message) {
            message.Add(Name);
            message.Add(Id);
            message.Add((ushort)Clients.Count);
            foreach (var item in Clients) {
                message.AddClass((T)item.Value);
            }
        }
        public virtual void Deserialize(IMessage message) { }
    }

}
