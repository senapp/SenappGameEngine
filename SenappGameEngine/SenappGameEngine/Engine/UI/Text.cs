using OpenTK;
using Senapp.Engine.Base;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer;
using Senapp.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.UI
{
    public class Text : Component
    {
        public List<double> textCharactersID = new List<double>();
        public List<double> textCharactersCustomID = new List<double>();
        public Vector3 colour = Vector3.One;
        public GameFont Font;
        public string text { get; private set; }
        public int fontSize { get; private set; }
        public int textLength { get; private set; }
        public int textHeight { get; private set; }

        public Text() { }
        public Text(string _text, GameFont _font, int _fontSize = 28)
        {
            Font = _font;
            fontSize = _fontSize;
            UpdateText(_text);
        }
        public void UpdateText(string _text)
        {
            textCharactersID.Clear();
            textCharactersCustomID.Clear();
            textLength = 0;

            text = _text;
            for (int n = 0; n < _text.Length; n++)
            {
                char idx = _text[n];
                var character = Font.GetCharacter(idx);

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

                    data.positions[4] = data.positions[1] - (float)character.height * fontSize / Transform.UIScalingConst;
                    data.positions[6] = data.positions[0] + (float)character.width * fontSize / Transform.UIScalingConst;
                    data.positions[7] = data.positions[1] - (float)character.height * fontSize / Transform.UIScalingConst;
                    data.positions[9] = data.positions[0] + (float)character.width * fontSize / Transform.UIScalingConst;
                    RawModel charData = Loader.LoadToVAO(data);

                    Font.characterRawModels.Add(characterID, charData);
                }

                textCharactersID.Add(character.id);
                textCharactersCustomID.Add(characterID);
                textLength += character.xadvance - 5;
                textHeight = Math.Max(textHeight, character.height + 5);
            }
        }
    }
}
