using actor;
using actor.module;
using game.actors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tp.slash {
    public class TP_SLASH_MASTER : MonoBehaviour {
        [SerializeField] private GameObject playerObject;

        private async void Start() {
            Cursor.lockState = CursorLockMode.Locked;
            var player = await ActorUltis.CreatePlayerAsync(playerObject);
            player.AddModule<UnityInputOldModule>(0);
            player.AddModule<LocomotionModule>(0);
            player.Initialize();
            player.AddCameraPlayer();
        }
    }
}
