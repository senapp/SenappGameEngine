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

namespace Senapp.Engine.UI.Combinations
{
    public class LabeledInputField : ComponentUI
    {
        public Text text;
        public InputField inputField;

        public LabeledInputField() { }
        public LabeledInputField(string label, string text, GameFont font, Dock dock) 
        {
            this.inputField = new InputField(text, font, dock);
            this.text = new Text(label, font, fontSize: defaultFontSize, dock: dock);
        }

        public override void Awake()
        {
            inputField = new GameObjectUI()
                .WithParent(gameObject)
                .WithName($"{gameObject.name} Labeled Input Field")
                .WithColour(defaultColour)
                .AddComponent(inputField);

            text = new GameObjectUI()
               .WithParent(gameObject)
               .WithPosition(new Vector3(defaultFontSize - 6, 10f, 0))
               .WithName($"{gameObject.name} Text")
               .AddComponent(text);

            SetSize(0.5f, new Vector2(0.9f, 0.25f));
        }

        public void SetSize(float size) => inputField.SetSize(size);
        public void SetSize(float size, Vector2 backgroundSize)
        {
            inputField.SetSize(size, backgroundSize);
            text.UpdateText(text.TextValue, defaultFontSize * size);
        }
        public void SetColour(Color labelTextColour, Color inputTextColour, Color backgroundColour)
        {
            inputField.SetColour(inputTextColour, backgroundColour);
            text.gameObject.colour = labelTextColour;
        }
        public void SetBackgroundColour(Color backgroundColour)
        {
            inputField.SetBackgroundColour(backgroundColour);
        }
        public void SetTextColour(Color labelTextColour, Color inputTextColour)
        {
            inputField.SetTextColour(inputTextColour);
            text.gameObject.colour = labelTextColour;
        }

        public LabeledInputField WithUIConstraint(UIPosition constraint)
        {
            this.UIConstriant = constraint;
            return this;
        }
        public LabeledInputField WithSortingLayer(int sortingLayer)
        {
            this.inputField.WithSortingLayer(sortingLayer);
            this.text.WithSortingLayer(sortingLayer);
            return this;
        }

        private const float defaultFontSize = 20f;
        private readonly Color defaultColour = Color.White;
    }
}
