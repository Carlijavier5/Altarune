using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace AllIn1SpriteShader
{
	public static class AllIn1InputSystem
	{
#if ENABLE_INPUT_SYSTEM
		public static bool GetKeyDown(KeyCode keyCode)
		{
			Key key = InputKeyConverter.GetKeyFromKeycode(keyCode);

			bool res = Keyboard.current[key].wasPressedThisFrame;
			return res;
		}

		public static bool GetKey(KeyCode keyCode)
		{
			Key key = InputKeyConverter.GetKeyFromKeycode(keyCode);
			bool res = Keyboard.current[key].isPressed;
			return res;
		}

		public static float GetMouseXAxis()
		{
			Vector2 delta = Mouse.current.delta.ReadValue();
			float res = delta.x * 0.1f;
			return res;
		}

		public static float GetMouseYAxis()
		{
			Vector2 delta = Mouse.current.delta.ReadValue();
			float res = delta.y * 0.1f;
			return res;
		}

		public static float GetMouseScroll()
		{
			Vector2 scrollValue = Mouse.current.scroll.ReadValue();
			
			float res = scrollValue.y * 0.1f;
			return res;
		}

#elif ENABLE_LEGACY_INPUT_MANAGER
		public static bool GetKeyDown(KeyCode keyCode)
		{
			bool res = Input.GetKeyDown(keyCode);
			return res;
		}

		public static bool GetKey(KeyCode keyCode)
		{
			bool res = Input.GetKey(keyCode);
			return res;
		}

		public static float GetMouseXAxis()
		{
			float res = Input.GetAxis("Mouse X");
			return res;
		}

		public static float GetMouseYAxis()
		{
			float res = Input.GetAxis("Mouse Y");
			return res;
		}

		public static float GetMouseScroll()
		{
			float res = Input.GetAxis("Mouse ScrollWheel");
			return res;
		}
#endif
	}
}
