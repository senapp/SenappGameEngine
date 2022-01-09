using System.Collections.Generic;

using OpenTK.Input;

namespace Senapp.Engine.PlayerInput
{
    public class ControllerManager
    {
        public static readonly int MAX_CONNECTED_CONTROLLERS = 4;
        public static readonly float JOYSTICK_MIN = 0.1f;

        private static List<Controller> activeControllers = new List<Controller>();
        public static Controller GetController(int id)
        {
            foreach (var controller in activeControllers)
            {
                if (controller.ActiveDevice == id) return controller;
            }
            return null;
        }
        public static bool ControllerExists(int id)
        {
            foreach (var controller in activeControllers)
            {
                if (controller.ActiveDevice == id) return true;
            }
            return false;
        }
        public static void Update()
        {
            GetActiveControllers();
            GetControllerInput();
        }
        private static void GetActiveControllers()
        {
            for (int i = 0; i < MAX_CONNECTED_CONTROLLERS; i++)
            {
                if (GamePad.GetState(i).IsConnected && !ControllerExists(i)) activeControllers.Add(new Controller(i));
                else if (!GamePad.GetState(i).IsConnected && ControllerExists(i)) activeControllers.RemoveAt(i);
            }
        }
        private static void GetControllerInput()
        {
            foreach (var controller in activeControllers)
            {
                controller.Update();
            }
        }
    }
}
