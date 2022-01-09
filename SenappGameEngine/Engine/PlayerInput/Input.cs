using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Input;

using Senapp.Engine.Base;

namespace Senapp.Engine.PlayerInput
{
    public class Input
    {
        private static List<Key> currentKeys = new List<Key>();
        private static List<Key> downKeys = new List<Key>();
        private static List<Key> upKeys = new List<Key>();
        private static List<MouseButton> currentMouseButtons = new List<MouseButton>();
        private static List<MouseButton> downMouseButtons= new List<MouseButton>();
        private static List<MouseButton> upMouseButtons = new List<MouseButton>();

        internal static void Update()
        {
            downKeys.Clear();
            for (int i = 0; i < Enum.GetNames(typeof(Key)).Length; i++)
            {
                if (GetKey((Key)i) && !currentKeys.Contains((Key)i))
                {
                    downKeys.Add((Key)i);
                }
            }
            upKeys.Clear();
            for (int i = 0; i < Enum.GetNames(typeof(Key)).Length; i++)
            {
                if (!GetKey((Key)i) && currentKeys.Contains((Key)i))
                {
                    upKeys.Add((Key)i);
                }
            }
            currentKeys.Clear();
            for (int i = 0; i < Enum.GetNames(typeof(Key)).Length; i++)
            {
                if (GetKey((Key)i))
                {
                    currentKeys.Add((Key)i);
                }
            }

            downMouseButtons.Clear();
            for (int i = 0; i < Enum.GetNames(typeof(MouseButton)).Length; i++)
            {
                if (GetMouseButton((MouseButton)i) && !currentMouseButtons.Contains((MouseButton)i))
                {
                    downMouseButtons.Add((MouseButton)i);
                }
            }
            upMouseButtons.Clear();
            for (int i = 0; i < Enum.GetNames(typeof(MouseButton)).Length; i++)
            {
                if (!GetMouseButton((MouseButton)i) && currentMouseButtons.Contains((MouseButton)i))
                {
                    upMouseButtons.Add((MouseButton)i);
                }
            }
            currentMouseButtons.Clear();
            for (int i = 0; i < Enum.GetNames(typeof(MouseButton)).Length; i++)
            {
                if (GetMouseButton((MouseButton)i))
                {
                    currentMouseButtons.Add((MouseButton)i);
                }
            }
            
        }
        public static bool GetKey(Key key)
        {
            if (!Game.Instance.Focused)
                return false;
            return Keyboard.GetState().IsKeyDown((short)key);
        }
        public static bool GetKeyDown(Key key)
        {
            if (!Game.Instance.Focused)
                return false;
            return downKeys.Contains(key);
        }
        public static bool GetKeyUp(Key key)
        {
            if (!Game.Instance.Focused)
                return false;
            return upKeys.Contains(key);
        }

        public static bool GetMouseButton(MouseButton mousebutton)
        {
            if (!Game.Instance.Focused)
                return false;
            return Mouse.GetState().IsButtonDown(mousebutton);
        }
        public static bool GetMouseButtonDown(MouseButton mousebutton)
        {
            if (!Game.Instance.Focused)
                return false;
            return downMouseButtons.Contains(mousebutton);
        }
        public static bool GetMouseButtonUp(MouseButton mousebutton)
        {
            if (!Game.Instance.Focused)
                return false;
            return upMouseButtons.Contains(mousebutton);
        }
        public static Vector2 GetMousePosition()
        {
            return new Vector2(Mouse.GetCursorState().X, Mouse.GetCursorState().Y);
        }
        public static Vector2 GetMousePositionScreen()
        {
            var vec = new Vector2(Mouse.GetCursorState().X - Game.Instance.X - Game.WINDOW_BORDER_SIZE, Mouse.GetCursorState().Y - Game.Instance.Y - (Game.WINDOW_BORDER_SIZE * 4 +2));
            vec = new Vector2(Math.Clamp(vec.X, 0, Game.Instance.Width), Math.Clamp(vec.Y, 0, Game.Instance.Height));
            return vec;
        }
        public static Vector2 GetMousePositionScreenCenter()
        {          
            int width = Game.Instance.Width / 2;
            int height = Game.Instance.Height / 2;
            var vec = new Vector2(Mouse.GetCursorState().X - Game.Instance.X - Game.WINDOW_BORDER_SIZE - width, Mouse.GetCursorState().Y - Game.Instance.Y - (Game.WINDOW_BORDER_SIZE * 4 + 2)- height);
            vec = new Vector2(Math.Clamp(vec.X, -width, width), Math.Clamp(vec.Y, -height, height));
            return vec;
        }
        private static Vector2 lastPos;
        public static Vector2 GetMouseDelta()
        {
            float deltaX = Mouse.GetCursorState().X - lastPos.X;
            float deltaY = Mouse.GetCursorState().Y - lastPos.Y;
            lastPos = new Vector2(Mouse.GetCursorState().X, Mouse.GetCursorState().Y);
            return new Vector2(deltaX, deltaY);
        }
        public static void SetMousePosition(Vector2 position)
        {
            Mouse.SetPosition(position.X, position.Y);
            lastPos = position;
        }
        public static void SetMousePositionScreen(Vector2 position)
        {
            position = new Vector2(Math.Clamp(position.X, 0, Game.Instance.Width), Math.Clamp(position.Y, 0, Game.Instance.Height));
            position = new Vector2(position.X + Game.Instance.X + Game.WINDOW_BORDER_SIZE, position.Y + Game.Instance.Y + (Game.WINDOW_BORDER_SIZE * 4 + 2));
            Mouse.SetPosition(position.X, position.Y);
            lastPos = position;
        }
        public static void SetMousePositionScreen(float x, float y)
        {
            SetMousePositionScreen(new Vector2(x, y));
        }
        public static void SetMousePositionScreenCenter(Vector2 position)
        {
            int width = Game.Instance.Width / 2;
            int height = Game.Instance.Height / 2;
            position = new Vector2(Math.Clamp(position.X, -width, width), Math.Clamp(position.Y, -height, height));
            position = new Vector2(position.X + Game.Instance.X + Game.WINDOW_BORDER_SIZE + width, position.Y + Game.Instance.Y + (Game.WINDOW_BORDER_SIZE * 4 + 2) + height);
            Mouse.SetPosition(position.X, position.Y);
            lastPos = position;
        }
        public static void SetMousePositionScreenCenter(float x , float y)
        {
            SetMousePositionScreenCenter(new Vector2(x, y));
        }
        public static void SetMousePosition(float x, float y)
        {
            Mouse.SetPosition(x, y);
        }
        public static void ShowCursor(bool visibility)
        {
            Game.Instance.CursorVisible = visibility;
        }
        public static bool GetCursorVisibility()
        {
            return Game.Instance.CursorVisible;
        }
        private static bool CursorLocked;
        public static void LockCursor(bool locked)
        {
            CursorLocked = locked;
        }
        public static bool GetLockCursor()
        {
            return CursorLocked;
        }

    }
}
