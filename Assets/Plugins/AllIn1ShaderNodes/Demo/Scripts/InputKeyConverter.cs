using UnityEngine;

namespace AllIn1SpriteShader
{
	public static class InputKeyConverter
	{
#if ENABLE_INPUT_SYSTEM
	public static UnityEngine.InputSystem.Key GetKeyFromKeycode(KeyCode keyCode)
	{
		UnityEngine.InputSystem.Key res = UnityEngine.InputSystem.Key.Space;
		switch (keyCode)
		{
			case KeyCode.A:
				res = UnityEngine.InputSystem.Key.A;
				break;
			case KeyCode.B:
				res = UnityEngine.InputSystem.Key.B;
				break;
			case KeyCode.C:
				res = UnityEngine.InputSystem.Key.C;
				break;
			case KeyCode.D:
				res = UnityEngine.InputSystem.Key.D;
				break;
			case KeyCode.E:
				res = UnityEngine.InputSystem.Key.E;
				break;
			case KeyCode.F:
				res = UnityEngine.InputSystem.Key.F;
				break;
			case KeyCode.G:
				res = UnityEngine.InputSystem.Key.G;
				break;
			case KeyCode.H:
				res = UnityEngine.InputSystem.Key.H;
				break;
			case KeyCode.I:
				res = UnityEngine.InputSystem.Key.I;
				break;
			case KeyCode.J:
				res = UnityEngine.InputSystem.Key.J;
				break;
			case KeyCode.K:
				res = UnityEngine.InputSystem.Key.K;
				break;
			case KeyCode.L:
				res = UnityEngine.InputSystem.Key.L;
				break;
			case KeyCode.M:
				res = UnityEngine.InputSystem.Key.M;
				break;
			case KeyCode.N:
				res = UnityEngine.InputSystem.Key.N;
				break;
			case KeyCode.O:
				res = UnityEngine.InputSystem.Key.O;
				break;
			case KeyCode.P:
				res = UnityEngine.InputSystem.Key.P;
				break;
			case KeyCode.Q:
				res = UnityEngine.InputSystem.Key.Q;
				break;
			case KeyCode.R:
				res = UnityEngine.InputSystem.Key.R;
				break;
			case KeyCode.S:
				res = UnityEngine.InputSystem.Key.S;
				break;
			case KeyCode.T:
				res = UnityEngine.InputSystem.Key.T;
				break;
			case KeyCode.U:
				res = UnityEngine.InputSystem.Key.U;
				break;
			case KeyCode.V:
				res = UnityEngine.InputSystem.Key.V;
				break;
			case KeyCode.W:
				res = UnityEngine.InputSystem.Key.W;
				break;
			case KeyCode.X:
				res = UnityEngine.InputSystem.Key.X;
				break;
			case KeyCode.Y:
				res = UnityEngine.InputSystem.Key.Y;
				break;
			case KeyCode.Z:
				res = UnityEngine.InputSystem.Key.Z;
				break;
			case KeyCode.Alpha0:
				res = UnityEngine.InputSystem.Key.Digit0;
				break;
			case KeyCode.Alpha1:
				res = UnityEngine.InputSystem.Key.Digit1;
				break;
			case KeyCode.Alpha2:
				res = UnityEngine.InputSystem.Key.Digit2;
				break;
			case KeyCode.Alpha3:
				res = UnityEngine.InputSystem.Key.Digit3;
				break;
			case KeyCode.Alpha4:
				res = UnityEngine.InputSystem.Key.Digit4;
				break;
			case KeyCode.Alpha5:
				res = UnityEngine.InputSystem.Key.Digit5;
				break;
			case KeyCode.Alpha6:
				res = UnityEngine.InputSystem.Key.Digit6;
				break;
			case KeyCode.Alpha7:
				res = UnityEngine.InputSystem.Key.Digit7;
				break;
			case KeyCode.Alpha8:
				res = UnityEngine.InputSystem.Key.Digit8;
				break;
			case KeyCode.Alpha9:
				res = UnityEngine.InputSystem.Key.Digit9;
				break;
			case KeyCode.Space:
				res = UnityEngine.InputSystem.Key.Space;
				break;
			case KeyCode.Return:
				res = UnityEngine.InputSystem.Key.Enter;
				break;
			case KeyCode.Escape:
				res = UnityEngine.InputSystem.Key.Escape;
				break;
			case KeyCode.Backspace:
				res = UnityEngine.InputSystem.Key.Backspace;
				break;
			case KeyCode.Tab:
				res = UnityEngine.InputSystem.Key.Tab;
				break;
			case KeyCode.LeftShift:
				res = UnityEngine.InputSystem.Key.LeftShift;
				break;
			case KeyCode.RightShift:
				res = UnityEngine.InputSystem.Key.RightShift;
				break;
			case KeyCode.LeftControl:
				res = UnityEngine.InputSystem.Key.LeftCtrl;
				break;
			case KeyCode.RightControl:
				res = UnityEngine.InputSystem.Key.RightCtrl;
				break;
			case KeyCode.LeftAlt:
				res = UnityEngine.InputSystem.Key.LeftAlt;
				break;
			case KeyCode.RightAlt:
				res = UnityEngine.InputSystem.Key.RightAlt;
				break;
			case KeyCode.LeftCommand:
				res = UnityEngine.InputSystem.Key.LeftCommand;
				break;
			case KeyCode.RightCommand:
				res = UnityEngine.InputSystem.Key.RightCommand;
				break;
			case KeyCode.LeftWindows:
				res = UnityEngine.InputSystem.Key.LeftWindows;
				break;
			case KeyCode.RightWindows:
				res = UnityEngine.InputSystem.Key.RightWindows;
				break;
			case KeyCode.UpArrow:
				res = UnityEngine.InputSystem.Key.UpArrow;
				break;
			case KeyCode.DownArrow:
				res = UnityEngine.InputSystem.Key.DownArrow;
				break;
			case KeyCode.LeftArrow:
				res = UnityEngine.InputSystem.Key.LeftArrow;
				break;
			case KeyCode.RightArrow:
				res = UnityEngine.InputSystem.Key.RightArrow;
				break;
			case KeyCode.F1:
				res = UnityEngine.InputSystem.Key.F1;
				break;
			case KeyCode.F2:
				res = UnityEngine.InputSystem.Key.F2;
				break;
			case KeyCode.F3:
				res = UnityEngine.InputSystem.Key.F3;
				break;
			case KeyCode.F4:
				res = UnityEngine.InputSystem.Key.F4;
				break;
			case KeyCode.F5:
				res = UnityEngine.InputSystem.Key.F5;
				break;
			case KeyCode.F6:
				res = UnityEngine.InputSystem.Key.F6;
				break;
			case KeyCode.F7:
				res = UnityEngine.InputSystem.Key.F7;
				break;
			case KeyCode.F8:
				res = UnityEngine.InputSystem.Key.F8;
				break;
			case KeyCode.F9:
				res = UnityEngine.InputSystem.Key.F9;
				break;
			case KeyCode.F10:
				res = UnityEngine.InputSystem.Key.F10;
				break;
			case KeyCode.F11:
				res = UnityEngine.InputSystem.Key.F11;
				break;
			case KeyCode.F12:
				res = UnityEngine.InputSystem.Key.F12;
				break;
			case KeyCode.Insert:
				res = UnityEngine.InputSystem.Key.Insert;
				break;
			case KeyCode.Delete:
				res = UnityEngine.InputSystem.Key.Delete;
				break;
			case KeyCode.Home:
				res = UnityEngine.InputSystem.Key.Home;
				break;
			case KeyCode.End:
				res = UnityEngine.InputSystem.Key.End;
				break;
			case KeyCode.PageUp:
				res = UnityEngine.InputSystem.Key.PageUp;
				break;
			case KeyCode.PageDown:
				res = UnityEngine.InputSystem.Key.PageDown;
				break;
			case KeyCode.Keypad0:
				res = UnityEngine.InputSystem.Key.Numpad0;
				break;
			case KeyCode.Keypad1:
				res = UnityEngine.InputSystem.Key.Numpad1;
				break;
			case KeyCode.Keypad2:
				res = UnityEngine.InputSystem.Key.Numpad2;
				break;
			case KeyCode.Keypad3:
				res = UnityEngine.InputSystem.Key.Numpad3;
				break;
			case KeyCode.Keypad4:
				res = UnityEngine.InputSystem.Key.Numpad4;
				break;
			case KeyCode.Keypad5:
				res = UnityEngine.InputSystem.Key.Numpad5;
				break;
			case KeyCode.Keypad6:
				res = UnityEngine.InputSystem.Key.Numpad6;
				break;
			case KeyCode.Keypad7:
				res = UnityEngine.InputSystem.Key.Numpad7;
				break;
			case KeyCode.Keypad8:
				res = UnityEngine.InputSystem.Key.Numpad8;
				break;
			case KeyCode.Keypad9:
				res = UnityEngine.InputSystem.Key.Numpad9;
				break;
			case KeyCode.KeypadDivide:
				res = UnityEngine.InputSystem.Key.NumpadDivide;
				break;
			case KeyCode.KeypadMultiply:
				res = UnityEngine.InputSystem.Key.NumpadMultiply;
				break;
			case KeyCode.KeypadMinus:
				res = UnityEngine.InputSystem.Key.NumpadMinus;
				break;
			case KeyCode.KeypadPlus:
				res = UnityEngine.InputSystem.Key.NumpadPlus;
				break;
			case KeyCode.KeypadEnter:
				res = UnityEngine.InputSystem.Key.NumpadEnter;
				break;
			case KeyCode.KeypadPeriod:
				res = UnityEngine.InputSystem.Key.NumpadPeriod;
				break;
			case KeyCode.CapsLock:
				res = UnityEngine.InputSystem.Key.CapsLock;
				break;
			case KeyCode.ScrollLock:
				res = UnityEngine.InputSystem.Key.ScrollLock;
				break;
			case KeyCode.Pause:
				res = UnityEngine.InputSystem.Key.Pause;
				break;
			case KeyCode.Quote:
				res = UnityEngine.InputSystem.Key.Quote;
				break;
			case KeyCode.Comma:
				res = UnityEngine.InputSystem.Key.Comma;
				break;
			case KeyCode.Minus:
				res = UnityEngine.InputSystem.Key.Minus;
				break;
			case KeyCode.Period:
				res = UnityEngine.InputSystem.Key.Period;
				break;
			case KeyCode.Slash:
				res = UnityEngine.InputSystem.Key.Slash;
				break;
			case KeyCode.Semicolon:
				res = UnityEngine.InputSystem.Key.Semicolon;
				break;
			case KeyCode.Equals:
				res = UnityEngine.InputSystem.Key.Equals;
				break;
			case KeyCode.LeftBracket:
				res = UnityEngine.InputSystem.Key.LeftBracket;
				break;
			case KeyCode.RightBracket:
				res = UnityEngine.InputSystem.Key.RightBracket;
				break;
			case KeyCode.Backslash:
				res = UnityEngine.InputSystem.Key.Backslash;
				break;
			case KeyCode.BackQuote:
				res = UnityEngine.InputSystem.Key.Backquote;
				break;
			default:
				res = UnityEngine.InputSystem.Key.Space;
				break;
		}

		return res;
	}
#endif
	}
}