using AllIn1SpriteShader;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace AllIn1ShaderNodes
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private KeyCode targetKey = KeyCode.A;
        [SerializeField] private KeyCode altKey = KeyCode.None;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        private Key targetNewInputKey;
        private Key altNewInputKey;
        
        private void Awake()
        {
            targetNewInputKey = InputKeyConverter.GetKeyFromKeycode(targetKey);
            altNewInputKey = InputKeyConverter.GetKeyFromKeycode(altKey);
        }
#endif

        public bool IsKeyPressed()
        {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if(altKey == KeyCode.None)
            {
                return Keyboard.current != null && Keyboard.current[targetNewInputKey].wasPressedThisFrame;
            }
            return Keyboard.current != null && (Keyboard.current[targetNewInputKey].wasPressedThisFrame || 
                                              Keyboard.current[altNewInputKey].wasPressedThisFrame);
#else
            if(altKey == KeyCode.None)
            {
                return Input.GetKeyDown(targetKey);
            }
            return Input.GetKeyDown(targetKey) || Input.GetKeyDown(altKey);
#endif
        }

        public KeyCode GetTargetKey() => targetKey;
        public KeyCode GetAltKey() => altKey;
    }
}