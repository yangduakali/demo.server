using actor;
using actor.module;
using UnityEngine;

namespace mrpg.server {
    public class InputReciverModule : ActorModule, IActorInput {
        public Vector2 moveInput;
        public Vector2 lookInput;
        public Quaternion cameraRotationn;

        public Quaternion CameraRotation() {
            return cameraRotationn;
        }

        public Vector2 LookAxis() {
            return lookInput;
        }

        public Vector2 MoveAxis() {
            return moveInput;
        }
    }
}
