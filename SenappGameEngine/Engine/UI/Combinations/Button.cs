using System;
using System.Drawing;

using OpenTK;

using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Raycasts;
using Senapp.Engine.UI.Components;

namespace Senapp.Engine.UI.Combinations
{
    public class Button : ComponentUI
    {
        public Sprite background;
        public RaycastTargetUI raycastTarget;

        public Button() { }
        public Button(string spriteTexture = "", Action onEnter = null, Action onClick = null, Action onExit = null, Action onLoseFocus = null) 
        {
            this.background = new Sprite(spriteTexture);
            raycastTarget = new RaycastTargetUI(onEnter, onClick, onExit, onLoseFocus);
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

            if (!sizeSet) SetSize(new Vector2(0.5f));
        }

        public void SetSize(Vector2 backgroundSize)
        {
            background.size = backgroundSize;
        }
        public void SetColour(Color colour)
        {
            background.gameObject.colour = colour;
        }

        public Button WithUIConstraint(UIPosition constraint)
        {
            this.UIConstriant = constraint;
            return this;
        }
        public Button WithSortingLayer(int sortingLayer)
        {
            this.background.SortingLayer = sortingLayer;
            return this;
        }
        public Button WithSize(Vector2 backgroundSize)
        {
            sizeSet = true;
            SetSize(backgroundSize);
            return this;
        }
        public Button WithColour(Color colour)
        {
            background.gameObject.colour = colour;
            return this;
        }

        private bool sizeSet = false;
        private readonly Color defaultColour = Color.White;
    }
}
