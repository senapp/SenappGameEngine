using System;
using System.Drawing;

using OpenTK;

using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Raycasts;
using Senapp.Engine.UI.Components;

namespace Senapp.Engine.UI.Combinations
{
    public class TextButton : ComponentUI
    {
        public Text text;
        public Sprite background;
        public RaycastTargetUI raycastTarget;

        public TextButton() { }
        public TextButton(string text, GameFont font, Action onEnter = null, Action onClick = null, Action onExit = null, Action onLoseFocus = null) 
        {
            this.text = new Text(text, font, defaultFontSize, Capitalization.Regularcase, Dock.Center);
            this.background = new Sprite();
            this.raycastTarget = new RaycastTargetUI(onEnter, onClick, onExit, onLoseFocus);
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

            SetSize(1, new Vector2(0.5f, 0.25f));
        }

        public void SetSize(float size) => SetSize(size, background.size);
        public void SetSize(float size, Vector2 backgroundSize)
        {
            gameObject.transform.SetScale(new Vector3(size));
            background.size = backgroundSize;
            text.UpdateText(text.TextValue, defaultFontSize * size);
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

        public TextButton WithUIConstraint(UIPosition constraint)
        {
            this.UIConstriant = constraint;
            return this;
        }
        public TextButton WithSortingLayer(int sortingLayer)
        {
            this.background.SortingLayer = sortingLayer;
            this.text.SortingLayer = sortingLayer;
            return this;
        }

        private readonly float defaultFontSize = 20;
        private readonly Color defaultColour = Color.White;
    }
}
