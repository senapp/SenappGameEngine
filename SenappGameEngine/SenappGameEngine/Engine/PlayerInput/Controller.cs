using OpenTK;
using OpenTK.Input;
using Senapp.Engine.Events;
using System;
using System.Collections.Generic;

namespace Senapp.Engine.PlayerInput
{
    public class Controller
    {
        public bool IsConnected = false;
        public string Name = "Controller";
        public int ActiveDevice = 0;

        private GamePadState gamepad;
        private JoystickState joystick;

        public Controller(int controllerID)
        {
            ActiveDevice = controllerID;
            gamepad = GamePad.GetState(ActiveDevice);
            joystick = Joystick.GetState(ActiveDevice);
        }
        public GamePadCapabilities CapabilitiesGamePad { get { return GamePad.GetCapabilities(ActiveDevice); } }
        public JoystickCapabilities CapabilitiesJoystick { get { return Joystick.GetCapabilities(ActiveDevice); } }

        public List<Buttons> currentButtons = new List<Buttons>();
        public List<Buttons> downButtons = new List<Buttons>();
        public List<Buttons> upButtons = new List<Buttons>();

        public void Update()
        {
            ActionEventArgs args = new ActionEventArgs(ActiveDevice);
            if (!args.GamePadState.Equals(gamepad))
            {
                gamepad = args.GamePadState;
            }
            if (!args.JoystickState.Equals(joystick))
            {
                joystick = args.JoystickState;
            }

            var buttonsValues = Enum.GetValues(typeof(Buttons));

            downButtons.Clear();
            for (int i = 0; i < buttonsValues.Length; i++)
            {
                if (GetButton((Buttons)buttonsValues.GetValue(i)) && !currentButtons.Contains((Buttons)buttonsValues.GetValue(i)))
                {
                    downButtons.Add((Buttons)buttonsValues.GetValue(i));
                }
            }
            upButtons.Clear();
            for (int i = 0; i < buttonsValues.Length; i++)
            {
                if (!GetButton((Buttons)buttonsValues.GetValue(i)) && currentButtons.Contains((Buttons)buttonsValues.GetValue(i)))
                {
                    upButtons.Add((Buttons)buttonsValues.GetValue(i));
                }
            }
            currentButtons.Clear();
            for (int i = 0; i < buttonsValues.Length; i++)
            {
                if (GetButton((Buttons)buttonsValues.GetValue(i)))
                {
                    currentButtons.Add((Buttons)buttonsValues.GetValue(i));
                }
            }
        }
        public Vector2 GetAxis(int xAxisIndex, int yAxisIndex)
        {
            var x = joystick.GetAxis(xAxisIndex);
            var y = joystick.GetAxis(yAxisIndex);

            if (x > ControllerManager.JOYSTICK_MIN || x < -ControllerManager.JOYSTICK_MIN || y > ControllerManager.JOYSTICK_MIN || y < -ControllerManager.JOYSTICK_MIN) return new Vector2(x, y);
            else return Vector2.Zero;
        }
        public bool GetButton(Buttons button)
        {
            if (!Game.Instance.Focused)
                return false;
            switch (button)
            {
                case Buttons.DPadUp:
                    return gamepad.DPad.Up == ButtonState.Pressed;
                case Buttons.DPadDown:
                    return gamepad.DPad.Down == ButtonState.Pressed;
                case Buttons.DPadLeft:
                    return gamepad.DPad.Left == ButtonState.Pressed;
                case Buttons.DPadRight:
                    return gamepad.DPad.Right == ButtonState.Pressed;
                case Buttons.Start:
                    return gamepad.Buttons.Start == ButtonState.Pressed;
                case Buttons.Back:
                    return gamepad.Buttons.Back == ButtonState.Pressed;
                case Buttons.LeftStick:
                    return gamepad.Buttons.LeftStick == ButtonState.Pressed;
                case Buttons.RightStick:
                    return gamepad.Buttons.RightStick == ButtonState.Pressed;
                case Buttons.LeftShoulder:
                    return gamepad.Buttons.LeftShoulder == ButtonState.Pressed;
                case Buttons.RightShoulder:
                    return gamepad.Buttons.RightShoulder == ButtonState.Pressed;
                case Buttons.BigButton:
                    return gamepad.Buttons.BigButton == ButtonState.Pressed;
                case Buttons.A:
                    return gamepad.Buttons.A == ButtonState.Pressed;
                case Buttons.B:
                    return gamepad.Buttons.B == ButtonState.Pressed;
                case Buttons.X:
                    return gamepad.Buttons.X == ButtonState.Pressed;
                case Buttons.Y:
                    return gamepad.Buttons.Y == ButtonState.Pressed;
                default:
                    return false;
            }
        }
        public bool GetButtonDown(Buttons button)
        {
            if (!Game.Instance.Focused)
                return false;
            return downButtons.Contains(button);
        }
        public bool GetButtonUp(Buttons button)
        {
            if (!Game.Instance.Focused)
                return false;
            return upButtons.Contains(button);
        }
    }
}
