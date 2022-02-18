using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Input;

using Senapp.Engine.Core;

namespace Senapp.Engine.PlayerInput
{
    public class Input
    {
        public static List<Key> CurrentKeys => currentKeys;
        public static List<MouseButton> CurrentMouseButtons => currentMouseButtons;

        public static bool CursorLocked { get; private set; }

        public static void Update()
        {
            downKeys.Clear();
            upKeys.Clear();
            for (int i = 0; i < Keys.Length; i++)
            {
                var key = (Key)i;
                var keyPressed = GetKeyBoardStateKey(key);

                if (keyPressed && !currentKeys.Contains(key))
                {
                    downKeys.Add(key);
                    currentKeys.Add(key);
                }
                else if (!keyPressed && currentKeys.Contains(key))
                {
                    upKeys.Add(key);
                    currentKeys.Remove(key);
                }
            }

            downMouseButtons.Clear();
            upMouseButtons.Clear();
            for (int i = 0; i < MouseButtons.Length; i++)
            {
                var mouseButton = (MouseButton)i;
                var mousePressed = GetMouseStateButton(mouseButton);

                if (mousePressed && !currentMouseButtons.Contains(mouseButton))
                {
                    downMouseButtons.Add(mouseButton);
                    currentMouseButtons.Add(mouseButton);
                }
                else if (!mousePressed && currentMouseButtons.Contains(mouseButton))
                {
                    upMouseButtons.Add(mouseButton);
                    currentMouseButtons.Remove(mouseButton);
                }
            }            
        }

        public static bool GetKey(Key key)
        {
            if (!Game.Instance.Focused)
                return false;

            return currentKeys.Contains(key);
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

            return currentMouseButtons.Contains(mousebutton);
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
        public static Vector2 GetMousePositionWindow()
        {
            var vec = new Vector2(Mouse.GetCursorState().X - Game.Instance.X - Game.WINDOW_BORDER_SIZE, Mouse.GetCursorState().Y - Game.Instance.Y - (Game.WINDOW_BORDER_SIZE * 4 +2));
            vec = new Vector2(Math.Clamp(vec.X, 0, Game.Instance.Width), Math.Clamp(vec.Y, 0, Game.Instance.Height));
            return vec;
        }
        public static Vector2 GetMousePositionWindowCenter()
        {          
            int width = Game.Instance.Width / 2;
            int height = Game.Instance.Height / 2;
            var vec = new Vector2(Mouse.GetCursorState().X - Game.Instance.X - Game.WINDOW_BORDER_SIZE - width, Mouse.GetCursorState().Y - Game.Instance.Y - (Game.WINDOW_BORDER_SIZE * 4 + 2)- height);
            vec = new Vector2(Math.Clamp(vec.X, -width, width), Math.Clamp(vec.Y, -height, height));
            return vec;
        }

        public static Vector2 GetMouseDelta()
        {
            float deltaX = Mouse.GetCursorState().X - lastPos.X;
            float deltaY = Mouse.GetCursorState().Y - lastPos.Y;
            lastPos = new Vector2(Mouse.GetCursorState().X, Mouse.GetCursorState().Y);
            return new Vector2(deltaX, deltaY);
        }
        public static bool IsMouseOnWindow(Vector2 mouseScreenPosition)
        {
            if (mouseScreenPosition.X <= 0 || mouseScreenPosition.X >= Game.Instance.Width || mouseScreenPosition.Y <= 0 || mouseScreenPosition.Y >= Game.Instance.Height)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void SetMousePosition(Vector2 position)
        {
            Mouse.SetPosition(position.X, position.Y);
            lastPos = position;
        }
        public static void SetMousePosition(float x, float y)
        {
            Mouse.SetPosition(x, y);
        }
        public static void SetMousePositionWindow(Vector2 position)
        {
            position = new Vector2(Math.Clamp(position.X, 0, Game.Instance.Width), Math.Clamp(position.Y, 0, Game.Instance.Height));
            position = new Vector2(position.X + Game.Instance.X + Game.WINDOW_BORDER_SIZE, position.Y + Game.Instance.Y + (Game.WINDOW_BORDER_SIZE * 4 + 2));
            Mouse.SetPosition(position.X, position.Y);
            lastPos = position;
        }
        public static void SetMousePositionWindow(float x, float y)
        {
            SetMousePositionWindow(new Vector2(x, y));
        }
        public static void SetMousePositionWindowCenter(Vector2 position)
        {
            int width = Game.Instance.Width / 2;
            int height = Game.Instance.Height / 2;
            position = new Vector2(Math.Clamp(position.X, -width, width), Math.Clamp(position.Y, -height, height));
            position = new Vector2(position.X + Game.Instance.X + Game.WINDOW_BORDER_SIZE + width, position.Y + Game.Instance.Y + (Game.WINDOW_BORDER_SIZE * 4 + 2) + height);
            Mouse.SetPosition(position.X, position.Y);
            lastPos = position;
        }
        public static void SetMousePositionWindowCenter(float x , float y)
        {
            SetMousePositionWindowCenter(new Vector2(x, y));
        }

        public static void ShowCursor(bool visibility)
        {
            Game.Instance.CursorVisible = visibility;
        }
        public static bool GetCursorVisibility()
        {
            return Game.Instance.CursorVisible;
        }
        public static void LockCursor(bool locked)
        {
            CursorLocked = locked;
        }

        private static bool GetKeyBoardStateKey(Key key)
        {
            if (!Game.Instance.Focused)
                return false;

            return Keyboard.GetState().IsKeyDown((short)key);
        }
        private static bool GetMouseStateButton(MouseButton mousebutton)
        {
            if (!Game.Instance.Focused)
                return false;

            return Mouse.GetState().IsButtonDown(mousebutton);
        }

        private static Vector2 lastPos;

        private static readonly string[] Keys = Enum.GetNames(typeof(Key));
        private static readonly string[] MouseButtons = Enum.GetNames(typeof(MouseButton));

        private static readonly List<Key> currentKeys = new();
        private static readonly List<MouseButton> currentMouseButtons = new();

        private static readonly List<Key> downKeys = new();
        private static readonly List<Key> upKeys = new();
        private static readonly List<MouseButton> downMouseButtons= new();
        private static readonly List<MouseButton> upMouseButtons = new();
    }
}
