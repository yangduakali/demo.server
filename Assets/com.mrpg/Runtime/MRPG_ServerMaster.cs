using ByteSizeLib;
using network.server;
using network.server.message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using value;
using ValueType = value.ValueType;

namespace mrpg.server {
    public class MRPG_ServerMaster : MonoBehaviour {

        [SerializeField] private ushort port = 8080;
        [SerializeField] private ushort maxClient = 100;
        [SerializeField] private ValueType prosessor;
        [SerializeField] private SceneTuple[] registerScene;
        [Space(20)]
        public UnityEvent<string> OnDebug;

        private IServerManager manager;
        private readonly Stopwatch stopwatch = new ();
        ByteSize sendBytes = new();
        ByteSize reciveBytes = new();
        private long highSend;
        private long highRecive;

        

       

        public struct P {
            public P(string n) {
                name = n;
                s = new();
            }
            public string name;
            public void Add(params P[] d) {
                s = d.ToList();
            }

            public List<P> s;

            public void Re() {
                print(name);
            }

            public void SendOther() {
                print(name);
                s.ForEach(x=> x.SendOther());
            }
        }

        private void Awake() {
            Application.runInBackground = true;
            manager = CreateManager(prosessor);
            manager.Port = port;
            manager.MaxClient = maxClient;
            manager.ValidateNetworkGroupHandler = ValidateNetworkGroupHandler;
        }

        private void Start() {
            manager.Start();
        }
        private void Update() {
            OnDebug?.Invoke(DebugText());
        }
        private void FixedUpdate() {
            manager.Tick();
        }
        private void OnApplicationQuit() {
            manager.Stop();
        }

        private IServerManager CreateManager(Type prossesorType) {
            var t = typeof(ServerManager<,,>);
            print(prossesorType);
            var ct = t.MakeGenericType(prossesorType,typeof(MRPG_NetworkEntity),typeof(NetworkScene<MRPG_NetworkEntity>));
            return Activator.CreateInstance(ct) as IServerManager;
        }
        private string DebugText() {
            string t =  "";
            if (manager.IsRunnning) {
                t += $"Is Running, Port: {manager.Port}";
                t += "\n";
                t += $"Conected client : {manager.ConnectedClient.Count}";
            }
            else {
                t += $"Not Running";
            }
            t += "\n";
            t += "\n";
            t += $"Message pool: {Message.pool.Count}";
            t += "\n";
            t += $"Message instance: {Message.instanceCount}";
            t += "\n";

            if (!stopwatch.IsRunning) {
                stopwatch.Start();
            }
            else {

                if(stopwatch.ElapsedMilliseconds >= 1000) {
                    if(ServerUltis.TotalByteSend > highSend) highSend = ServerUltis.TotalByteSend;
                    if (ServerUltis.TotalByteRecive > highRecive) highRecive = ServerUltis.TotalByteRecive;

                    sendBytes = ByteSize.FromBits(ServerUltis.TotalByteSend);
                    reciveBytes = ByteSize.FromBits(ServerUltis.TotalByteRecive);

                    ServerUltis.TotalByteSend = 0;
                    ServerUltis.TotalByteRecive = 0;
                    stopwatch.Reset();
                }
            }

            t += $"Recive bytes: {reciveBytes}";
            t += "\n";
            t += $"High recive: {ByteSize.FromBits(highRecive)}";
            t += "\n";
            t += $"Send bytes: {sendBytes}";
            t += "\n";
            t += $"High send: {ByteSize.FromBits(highSend)}";
            return t;
        }
        private bool ValidateNetworkGroupHandler(string name, ushort id, out INetworkGroup networkGroup) {
            networkGroup = null;
            for (int i = 0; i < registerScene.Length; i++) {
                if (registerScene[i].scene == name) {
                    networkGroup = new NetworkScene<MRPG_NetworkEntity>() {
                        Name = name,
                        IsInstance = registerScene[i].isInstance
                    };
                    return true;
                }
            }
            return false;
        }


        [System.Serializable]
        public struct SceneTuple {
            public string scene;
            public bool isInstance;
        }
    }

}



/*
 
 
 
 */