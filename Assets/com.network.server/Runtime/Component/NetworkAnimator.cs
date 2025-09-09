using network.server.message;
using UnityEngine;

namespace network.server.component {
    [AddComponentMenu("Com/Network/Server/Server Animator")]
    public class NetworkAnimator : NetworkComponent<Animator> {

        private AnimatorControllerParameter[] parameters;
        private int parametersCount;

        public override void Initialize(string identifier, Animator component = null) {
            parameters = component.parameters;
            parametersCount = component.parameterCount;

            base.Initialize(identifier, component);
        }

        protected override void OnSend(IMessage message) {
            for (int i = 0; i < parametersCount; i++) {
                var parameter = parameters[i];
                switch (parameter.type) {
                    case AnimatorControllerParameterType.Float:
                        message.Add(component.GetFloat(parameter.name));
                        break;
                    case AnimatorControllerParameterType.Int:
                        message.Add(component.GetInteger(parameter.name));
                        break;
                    case AnimatorControllerParameterType.Bool:
                        message.Add(component.GetBool(parameter.name));
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
