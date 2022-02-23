using System;
using System.Collections.Generic;

using OpenTK;

using Senapp.Engine.Core;
using Senapp.Engine.Core.Transforms;
using Senapp.Engine.Loaders;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer.Helper;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.UI.Components
{
    public enum Capitalization
    {
        Regularcase,
        Uppercase,
        Lowercase,
        None
    }

    public enum Dock
    {
        Left,
        Center,
        Right,
        None
    }

    public class Text : ComponentUI
    {
        public const float FontScalingDivisor = 10000;
        public const float DefaultFontSize = 28;

        public string TextValue { get; private set; }
        public float CharacterSpacing { get; private set; }
        public float RawFontSize { get; private set; }
        public Capitalization Capitalization { get; private set; } = Capitalization.Regularcase;
        public Dock Dock { get; private set; } = Dock.Left;

        public GameFont font;

        public Text() { }
        public Text(string text, GameFont font, float fontSize = DefaultFontSize, Capitalization capitalization = Capitalization.Regularcase, Dock dock = Dock.Left)
        {
            this.font = font;
            this.RawFontSize = fontSize / FontScalingDivisor;
            this.Capitalization = capitalization;
            this.Dock = dock;

            UpdateText(text);
        }
        
        public float GetFontSize()
        {
            return RawFontSize * FontScalingDivisor;
        }
        public void UpdateText(string text, float fontSize = 0, Capitalization capitalization = Capitalization.None, Dock dock = Dock.None)
        {
            if (text == TextValue && fontSize == 0 && capitalization == Capitalization.None && dock == Dock.None) return;

            TextCharactersId.Clear();
            TextCharactersCustomId.Clear();
            TextRenderHeightsOffsets.Clear();
            TextRenderLengths.Clear();
            TextLength = 0;

            TextValue = text;


            if (fontSize != 0)
            {
                this.RawFontSize = fontSize / FontScalingDivisor;
            }
            if (capitalization != Capitalization.None)
            {
                Capitalization = capitalization;
                switch (Capitalization)
                {
                    case Capitalization.Regularcase:
                        TextValue = text;
                        break;
                    case Capitalization.Uppercase:
                        TextValue = text.ToUpper();
                        break;
                    case Capitalization.Lowercase:
                        TextValue = text.ToLower();
                        break;
                }
            }
            if (dock != Dock.None)
            {
                Dock = dock;
            }

            for (int n = 0; n < TextValue.Length; n++)
            {
                char idx = TextValue[n];
                var character = font.GetCharacter(idx);
                if (character == null) 
                    character = font.GetCharacter('?');
                var characterID = Mathematics.UniqueCombine(character.id, this.RawFontSize);
                LoaderVertex data = Geometry.GetVertex(Geometries.Quad);

                if (!font.characterRawModels.TryGetValue(characterID, out RawModel model))
                {
                    data.textureCoords[0] = (float)character.x / font.fontAtlas.Width;
                    data.textureCoords[1] = (float)character.y / font.fontAtlas.Height;

                    data.textureCoords[2] = (float)character.x / font.fontAtlas.Width;
                    data.textureCoords[3] = (float)(character.y + character.height) / font.fontAtlas.Height;

                    data.textureCoords[4] = (float)(character.x + character.width) / font.fontAtlas.Width;
                    data.textureCoords[5] = (float)(character.y + character.height) / font.fontAtlas.Height;

                    data.textureCoords[6] = (float)(character.x + character.width) / font.fontAtlas.Width;
                    data.textureCoords[7] = (float)character.y / font.fontAtlas.Height;

                    data.positions[4] = data.positions[1] - (float)character.height * this.RawFontSize;
                    data.positions[6] = data.positions[0] + (float)character.width * this.RawFontSize;
                    data.positions[7] = data.positions[1] - (float)character.height * this.RawFontSize;
                    data.positions[9] = data.positions[0] + (float)character.width * this.RawFontSize;
                    RawModel charData = Loader.LoadToVAO(data, $"Character_{font.file}_{characterID}");

                    font.characterRawModels.Add(characterID, charData);
                }

                TextCharactersId.Add(character.id);
                TextCharactersCustomId.Add(characterID);

                TextRenderLengths.Add(character.xoffset + TextLength);
                TextRenderHeightsOffsets.Add(character.yoffset);

                TextLength += character.xadvance - (int)CharacterSpacing;
                TextHeight = Math.Max(TextHeight, character.height);
            }
            for (int i = 0; i < TextRenderLengths.Count; i++)
            {
                switch (Dock)
                {
                    case Dock.Center:
                        TextRenderLengths[i] -= TextLength / 2;
                        break;
                    case Dock.Left:
                        break;
                    case Dock.Right:
                        TextRenderLengths[i] -= TextLength;
                        break;
                }
            }
            for (int i = 0; i < TextRenderHeightsOffsets.Count; i++)
            {
                TextRenderHeightsOffsets[i] -= TextOffset;
            }

            if (gameObject != null) gameObject.IsGameObjectUpdated = true;
        }

        public override Vector4 GetUIDimensionsPixels()
        {
            if (gameObject.IsGameObjectUpdated)
            {
                gameObject.IsGameObjectUpdated = false;
            }
            else
            {
                return dimensions;
            }

            Vector2 screenSize = new(Game.Instance.Width, Game.Instance.Height);
            var position = gameObject.transform.GetWorldPosition();
            var scale = gameObject.transform.GetWorldScale();

            position = new Vector3(position.X + Game.Instance.AspectRatio - (0.5f * scale.X), -(position.Y -TextOffset / Transform.UIScalingDivisor - 1 + 0.5f * scale.Y) - 30 * RawFontSize, position.Z);

            var xLength = Game.Instance.AspectRatio * 2;
            var yLength = 2;

            var xVal = position.X / xLength;
            var yVal = position.Y / yLength;

            var xTextLength = (int)(screenSize.X * (TextLength * RawFontSize / xLength));
            var yTextLength = (int)(screenSize.Y * (TextHeight * RawFontSize / yLength));

            return dimensions = new Vector4((int)(screenSize.X * xVal), (int)(screenSize.Y * yVal), (int)(screenSize.X * xVal + xTextLength), (int)(screenSize.Y * yVal + yTextLength));
        }

        public Text WithUIConstraint(UIPosition constraint)
        {
            this.UIConstriant = constraint;
            return this;
        }
        public Text WithSortingLayer(int sortingLayer)
        {
            this.SortingLayer = sortingLayer;
            return this;
        }

        #region Rendering
        public List<double> TextCharactersId { get; private set; } = new List<double>();
        public List<double> TextCharactersCustomId { get; private set; } = new List<double>();
        public List<float> TextRenderLengths { get; private set; } = new List<float>();
        public List<float> TextRenderHeightsOffsets { get; private set; } = new List<float>();
        public float TextLength { get; private set; }
        public float TextHeight { get; private set; }
        #endregion
    }
}
