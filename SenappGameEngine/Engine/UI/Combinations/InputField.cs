using System;
using System.Drawing;
using System.Linq;

using OpenTK;
using OpenTK.Input;

using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Events;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.Raycasts;
using Senapp.Engine.UI.Components;

using static Senapp.Engine.PlayerInput.InputExtensions;

namespace Senapp.Engine.UI.Combinations
{
    public class InputField : ComponentUI
    {
        public string TextValue;

        public Text text;
        public Sprite background;
        public RaycastTargetUI raycastTarget;
        private Sprite currentPosMarker;

        public InputField() { }
        public InputField(string text, GameFont font, Dock dock) 
        {
            this.text = new Text(text, font, defaultFontSize, Capitalization.Regularcase, dock);
            this.background = new Sprite();
            this.currentPosMarker = new Sprite();
            this.TextValue = text;
            this.raycastTarget = new RaycastTargetUI(onClick: OnFocus, onLoseFocus: OnLoseFocus);
        }

        public override void Awake()
        {
            background = new GameObjectUI()
                .WithParent(gameObject)
                .WithName($"{gameObject.name} Background")
                .WithColour(defaultColour)
                .WithComponent(background)
                .WithComponent(raycastTarget)
                .GetComponent<Sprite>();

            text = new GameObjectUI()
               .WithParent(gameObject)
               .WithPosition(new Vector3(TextOffset, 0, 0))
               .WithName($"{gameObject.name} Text")
               .AddComponent(text);

            currentPosMarker = new GameObjectUI()
                .WithParent(gameObject)
                .WithName($"{gameObject.name} Marker")
                .WithEnable(false)
                .WithColour(Color.Black)
                .AddComponent(currentPosMarker)
                .WithSortingLayer(text.SortingLayer + 1);

            SetSize(0.5f, new Vector2(0.9f, 0.25f));
        }

        public override void Update(GameUpdatedEventArgs args)
        {
            if (lisiting)
            {
                if (Input.CurrentKeys.Any())
                {
                    HandleKey();
                }
                else
                {
                    lastKey = Key.Unknown;
                }
            }

            if (inputUpdateDeltaTime + args.DeltaTime > inputUpdateFrequency)
            {
                inputUpdateDeltaTime = 0;
                lastKey = Key.Unknown;
                if (lisiting)
                {
                    UpdateCurrentPosMarker();
                    currentPosMarker.gameObject.enabled = !currentPosMarker.gameObject.enabled;
                }
            }
            else
            {
                inputUpdateDeltaTime += args.DeltaTime;
            }
        }

        public void SetSize(float size) => SetSize(size, background.size);
        public void SetSize(float size, Vector2 backgroundSize)
        {
            gameObject.transform.SetScale(new Vector3(size));
            background.size = backgroundSize;
            text.UpdateText(text.TextValue, defaultFontSize * size);
            switch (text.Dock)
            {
                case Dock.Left:
                    text.gameObject.transform.SetPosition(new Vector3(TextOffset - defaultFontSize * size * mult * backgroundSize.X, 0, 0));
                    break;
                case Dock.None:
                case Dock.Center:
                    text.gameObject.transform.SetPosition(new Vector3(TextOffset, 0, 0));
                    break;
                case Dock.Right:
                    text.gameObject.transform.SetPosition(new Vector3(TextOffset + defaultFontSize * size * mult * backgroundSize.X, 0, 0));
                    break;
            }
            currentPosMarker.size = new Vector2(size / 40f, backgroundSize.Y / 2);
        }
        public void SetColour(Color textColour, Color backgroundColour)
        {
            text.gameObject.colour = textColour;
            background.gameObject.colour = backgroundColour;
        }
        public void SetBackgroundColour(Color backgroundColour)
        {
            background.gameObject.colour = backgroundColour;
        }
        public void SetTextColour(Color textColour)
        {
            text.gameObject.colour = textColour;
        }

        public InputField WithUIConstraint(UIPosition constraint)
        {
            this.UIConstriant = constraint;
            return this;
        }
        public InputField WithSortingLayer(int sortingLayer)
        {
            this.background.SortingLayer = sortingLayer;
            this.text.SortingLayer = sortingLayer;
            this.currentPosMarker.SortingLayer = sortingLayer + 1;
            return this;
        }

        private void HandleKey()
        {
            var count = Input.CurrentKeys.Count;
            var shiftOn = count > 1 && (Input.CurrentKeys[count - 2] == Key.ShiftLeft || Input.CurrentKeys[count - 2] == Key.ShiftRight);
            var altGrOn = count > 1 && (Input.CurrentKeys[count - 2] == Key.AltRight);
            var key = Input.CurrentKeys.Last();

            if (key == lastKey) return;
            lastKey = key;

            var value = TextValue;
            var added = key.ConvertToString(shiftOn, altGrOn);
            if (string.IsNullOrEmpty(added))
            {
                if (key == Key.BackSpace && TextValue.Length > 0)
                {
                    value = TextValue.Remove(currentPosition - 1, 1);
                    currentPosition--;
                }
                else if (key == Key.Left)
                {
                    currentPosition = Math.Max(0, currentPosition - 1);
                }
                else if (key == Key.Right)
                {
                    currentPosition = Math.Min(value.Length, currentPosition + 1);
                }
            }
            else
            {
                value = TextValue.Insert(currentPosition, added);
                currentPosition++;
            }

            if (TextValue != value)
            {
                TextValue = value;
                text.UpdateText(value);
                UpdateCurrentPosMarker();
            }
        }
        private void UpdateCurrentPosMarker()
        {
            var offset = 0f;
            if (text.TextRenderLengths.Count > 0)
            {
                if (currentPosition < text.TextRenderLengths.Count)
                {
                    offset = text.TextRenderLengths[currentPosition];
                }
                else if (text.Dock == Dock.Left)
                {
                    offset = text.TextLength;
                }
                else if (text.Dock == Dock.Center)
                {
                    offset = text.TextLength / 2;
                }
            }
            switch (text.Dock)
            {
                case Dock.Left:
                    offset -= 7.5f;
                    currentPosMarker.gameObject.transform.SetPosition(new Vector3(-TextOffset * background.size.X / mult + offset / 10, 0, 0));
                    break;
                case Dock.None:
                case Dock.Center:
                    currentPosMarker.gameObject.transform.SetPosition(new Vector3(offset / 10, 0, 0));
                    break;
                case Dock.Right:
                    offset += 12.5f;
                    currentPosMarker.gameObject.transform.SetPosition(new Vector3(TextOffset * background.size.X / mult + offset / 10, 0, 0));
                    break;
            }
        }

        private void OnFocus() 
        { 
            lisiting = true; 
            currentPosition = text.TextValue.Length; 
        }
        private void OnLoseFocus()
        {
            lisiting = false;
            currentPosMarker.gameObject.enabled = false;
        }

        private int currentPosition;
        private bool lisiting;
        private Key lastKey = Key.Unknown;

        private float inputUpdateDeltaTime = 0;
        private const float inputUpdateFrequency = 0.5f; // 2 updates per second

        private readonly float defaultFontSize = 20;
        private readonly Color defaultColour = Color.White;
        private const float mult = 2.3f;
    }
}
