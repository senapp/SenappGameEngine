using OpenTK.Input;

namespace Senapp.Engine.PlayerInput
{
    public static class InputExtensions
    {
        public static string ConvertToString(this Key key, bool shiftOn, bool altGrOn)
        {
            switch (key)
            {
                case Key.Keypad0:
                case Key.Keypad1:
                case Key.Keypad2:
                case Key.Keypad3:
                case Key.Keypad4:
                case Key.Keypad5:
                case Key.Keypad6:
                case Key.Keypad7:
                case Key.Keypad8:
                case Key.Keypad9:
                case Key.Number0:
                case Key.Number1:
                case Key.Number2:
                case Key.Number3:
                case Key.Number4:
                case Key.Number5:
                case Key.Number6:
                case Key.Number7:
                case Key.Number8:
                case Key.Number9:
                    return KeyToNumber(key, shiftOn, altGrOn);
                case Key.A:
                case Key.B:
                case Key.C:
                case Key.D:
                case Key.E:
                case Key.F:
                case Key.G:
                case Key.H:
                case Key.I:
                case Key.J:
                case Key.K:
                case Key.L:
                case Key.M:
                case Key.N:
                case Key.O:
                case Key.P:
                case Key.Q:
                case Key.R:
                case Key.S:
                case Key.T:
                case Key.U:
                case Key.V:
                case Key.W:
                case Key.X:
                case Key.Y:
                case Key.Z:
                    return shiftOn ? key.ToString() : key.ToString().ToLower();
                case Key.Minus:
                    return shiftOn ? "_" : altGrOn ? "" : "-";
                case Key.Plus:
                    return shiftOn ? "?": altGrOn ? "\\" : "+";
                case Key.Comma:
                    return shiftOn ? ";" : altGrOn ? "" : ",";
                case Key.Period:
                    return shiftOn ? ":" : altGrOn ? "" : ".";
                case Key.Space:
                    return shiftOn ? " " : altGrOn ? "" : " ";
                default:
                    return string.Empty;
            }
        }
        public static string KeyToNumber(this Key key, bool shiftOn, bool altGrOn)
        {
            switch (key)
            {
                case Key.Keypad0:
                case Key.Number0:
                    return shiftOn ? "=" : altGrOn ? "}" : "0";
                case Key.Keypad1:
                case Key.Number1:
                    return shiftOn ? "!" : altGrOn ? "" : "1";
                case Key.Keypad2:
                case Key.Number2:
                    return shiftOn ? "\"" : altGrOn ? "@" : "2";
                case Key.Keypad3:
                case Key.Number3:
                    return shiftOn ? "#" : altGrOn ? "£" : "3";
                case Key.Keypad4:
                case Key.Number4:
                    return shiftOn ? "¤" : altGrOn ? "$" : "4";
                case Key.Keypad5:
                case Key.Number5:
                    return shiftOn ? "%" : altGrOn ? "€" : "5";
                case Key.Keypad6:
                case Key.Number6:
                    return shiftOn ? "&" : altGrOn ? "" : "6";
                case Key.Keypad7:
                case Key.Number7:
                    return shiftOn ? "/" : altGrOn ? "{" : "7";
                case Key.Keypad8:
                case Key.Number8:
                    return shiftOn ? "(" : altGrOn ? "[" : "8";
                case Key.Keypad9:
                case Key.Number9:
                    return shiftOn ? ")" : altGrOn ? "]" : "9";
                default:
                    return string.Empty;
            }
        }
    }
}
