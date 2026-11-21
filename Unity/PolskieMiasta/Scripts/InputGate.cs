using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Miasta
{
    public static class InputGate
    {
        public static bool GetKey(KeyCode k)
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb == null) return false;
            var key = Map(k);
            if (key == Key.None) return false;
            return kb[key].isPressed;
#else
            return Input.GetKey(k);
#endif
        }

        public static bool GetKeyDown(KeyCode k)
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb == null) return false;
            var key = Map(k);
            if (key == Key.None) return false;
            return kb[key].wasPressedThisFrame;
#else
            return Input.GetKeyDown(k);
#endif
        }

        public static float MouseX()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current == null) return 0f;
            return Mouse.current.delta.ReadValue().x * 0.25f;
#else
            return Input.GetAxisRaw("Mouse X");
#endif
        }

        public static float MouseY()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current == null) return 0f;
            return Mouse.current.delta.ReadValue().y * 0.25f;
#else
            return Input.GetAxisRaw("Mouse Y");
#endif
        }

        public static bool GetMouseButton(int b)
        {
#if ENABLE_INPUT_SYSTEM
            var m = Mouse.current;
            if (m == null) return false;
            if (b == 0) return m.leftButton.isPressed;
            if (b == 1) return m.rightButton.isPressed;
            if (b == 2) return m.middleButton.isPressed;
            return false;
#else
            return Input.GetMouseButton(b);
#endif
        }

        public static bool TypingInField()
        {
            var es = UnityEngine.EventSystems.EventSystem.current;
            var sel = es != null ? es.currentSelectedGameObject : null;
            return sel != null && sel.GetComponent<UnityEngine.UI.InputField>() != null;
        }

#if ENABLE_INPUT_SYSTEM
        static Key Map(KeyCode k)
        {
            switch (k)
            {
                case KeyCode.W: return Key.W;
                case KeyCode.A: return Key.A;
                case KeyCode.S: return Key.S;
                case KeyCode.D: return Key.D;
                case KeyCode.E: return Key.E;
                case KeyCode.H: return Key.H;
                case KeyCode.G: return Key.G;
                case KeyCode.F: return Key.F;
                case KeyCode.R: return Key.R;
                case KeyCode.T: return Key.T;
                case KeyCode.Escape: return Key.Escape;
                case KeyCode.LeftShift: return Key.LeftShift;
                case KeyCode.Space: return Key.Space;
                default: return Key.None;
            }
        }
#endif
    }
}
