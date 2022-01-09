using System;
using System.Collections.Generic;

using Senapp.Engine.Base;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.UI
{
    public class Text : UIElement
    {
        public static readonly float TextYOffset = 6;

        public GameFont Font;

        public List<double> textCharactersID = new List<double>();
        public List<double> textCharactersCustomID = new List<double>();

        public List<float> textRenderLengths = new List<float>();
        public List<float> textRenderHeightsOffsets = new List<float>();

        public string text { get; private set; }
        public Capitalization textCapitalzation = Capitalization.Regularcase;

        public int fontSize { get; private set; }
        public int textLength { get; private set; }
        public int textHeight { get; private set; }

        public enum Capitalization
        {
            Regularcase,
            Uppercase,
            Lowercase,
        }

        public Text() { }
        public Text(string _text, GameFont _font, int _fontSize = 28, Capitalization capitalization = Capitalization.Regularcase)
        {
            Font = _font;
            fontSize = _fontSize;
            textCapitalzation = capitalization;
            UpdateText(_text);
        }
        public void UpdateText(string _text)
        {
            textCharactersID.Clear();
            textCharactersCustomID.Clear();
            textRenderHeightsOffsets.Clear();
            textRenderLengths.Clear();
            textLength = 0;

            switch (textCapitalzation)
            {
                case Capitalization.Regularcase:
                    text = _text;
                    break;
                case Capitalization.Uppercase:
                    text = _text.ToUpper();
                    break;
                case Capitalization.Lowercase:
                    text = _text.ToLower();
                    break;
            }
            for (int n = 0; n < text.Length; n++)
            {
                char idx = text[n];
                var character = Font.GetCharacter(idx);
                if (character == null) 
                    character = Font.GetCharacter('?');
                var characterID = Mathematics.UniqueCombine(character.id, fontSize);
                LoaderVertex data = Geometry.GetVertex(Geometries.Quad);

                if (!Font.characterRawModels.TryGetValue(characterID, out RawModel model))
                {
                    data.textureCoords[0] = (float)character.x / Font.fontAtlas.Width;
                    data.textureCoords[1] = (float)character.y / Font.fontAtlas.Height;

                    data.textureCoords[2] = (float)character.x / Font.fontAtlas.Width;
                    data.textureCoords[3] = (float)(character.y + character.height) / Font.fontAtlas.Height;

                    data.textureCoords[4] = (float)(character.x + character.width) / Font.fontAtlas.Width;
                    data.textureCoords[5] = (float)(character.y + character.height) / Font.fontAtlas.Height;

                    data.textureCoords[6] = (float)(character.x + character.width) / Font.fontAtlas.Width;
                    data.textureCoords[7] = (float)character.y / Font.fontAtlas.Height;

                    data.positions[4] = data.positions[1] - (float)character.height * fontSize / Sprite.UIScalingConst;
                    data.positions[6] = data.positions[0] + (float)character.width * fontSize / Sprite.UIScalingConst;
                    data.positions[7] = data.positions[1] - (float)character.height * fontSize / Sprite.UIScalingConst;
                    data.positions[9] = data.positions[0] + (float)character.width * fontSize / Sprite.UIScalingConst;
                    RawModel charData = Loader.LoadToVAO(data, $"Character_{Font.file}_{characterID}");

                    Font.characterRawModels.Add(characterID, charData);
                }

                textCharactersID.Add(character.id);
                textCharactersCustomID.Add(characterID);

                textRenderLengths.Add(character.xoffset + textLength);
                textRenderHeightsOffsets.Add(character.yoffset);

                textLength += character.xadvance - 5;
                textHeight = Math.Max(textHeight, character.height + 5);
            }
        }
    }
}
