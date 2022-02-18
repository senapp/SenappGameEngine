using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Input;

using Senapp.Engine.Core;
using Senapp.Engine.Events;

namespace Senapp.Engine.PlayerInput
{
    public enum Axis 
    { 
        HorizontalLeft, 
        VerticalLeft, 
        HorizontalRight, 
        VerticalRight 
    }

    public class Controller
    {
        public const float JOYSTICK_MIN = 0.1f;

        public static readonly string[] ControllerButtons = Enum.GetNames(typeof(Buttons));

        public GamePadCapabilities CapabilitiesGamePad => GamePad.GetCapabilities(ActiveDevice);
        public JoystickCapabilities CapabilitiesJoystick => Joystick.GetCapabilities(ActiveDevice);

        public List<Buttons> currentButtons = new();
        public List<Buttons> downButtons = new();
        public List<Buttons> upButtons = new();

        public bool IsConnected;
        public string Name;
        public int ActiveDevice;

        public Controller(int controllerID)
        {
            ActiveDevice = controllerID;
            Name = $"Controller - {ActiveDevice}";
            gamepad = GamePad.GetState(ActiveDevice);
            joystick = Joystick.GetState(ActiveDevice);
        }

        public void Update()
        {
            ActionEventArgs args = new(ActiveDevice);
            if (!args.GamePadState.Equals(gamepad))
            {
                gamepad = args.GamePadState;
            }
            if (!args.JoystickState.Equals(joystick))
            {
                joystick = args.JoystickState;
            }

            downButtons.Clear();
            upButtons.Clear();
            for (int i = 0; i < ControllerButtons.Length; i++)
            {
                var button = (Buttons)i;
                var buttonPressed = GetButtonOnController(button);

                if (buttonPressed && !currentButtons.Contains(button))
                {
                    downButtons.Add(button);
                    currentButtons.Add(button);
                }
                else if (!buttonPressed && currentButtons.Contains(button))
                {
                    upButtons.Add(button);
                    currentButtons.Remove(button);
                }
            }
        }

        public float GetAxis(Axis axis)
        {
            switch (axis)
            {
                case Axis.HorizontalLeft:
                    return GetAxis(0, 0).X;
                case Axis.VerticalLeft:
                    return -GetAxis(1, 1).Y;
                case Axis.HorizontalRight:
                    return GetAxis(3, 3).X;
                case Axis.VerticalRight:
                    return GetAxis(4, 4).Y;
                default:
                    return 0;
            }
        }
        public Vector2 GetAxis(int xAxisIndex, int yAxisIndex)
        {
            var x = joystick.GetAxis(xAxisIndex);
            var y = joystick.GetAxis(yAxisIndex);

            float nX = 0;
            float nY = 0;

            if (x > JOYSTICK_MIN || x < -JOYSTICK_MIN) nX = x;
            if (y > JOYSTICK_MIN || y < -JOYSTICK_MIN) nY = y;

            return new Vector2(nX, nY);
        }

        public bool GetButton(Buttons button)
        {
            if (!Game.Instance.Focused)
                return false;

            return currentButtons.Contains(button);
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

        private bool GetButtonOnController(Buttons button)
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

        private GamePadState gamepad;
        private JoystickState joystick;
    }
}
