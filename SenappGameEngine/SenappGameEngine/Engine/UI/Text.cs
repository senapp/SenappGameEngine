using Senapp.Engine.Base;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.UI
{
    public class Text : Component
    {
        public static readonly float ScalingConst = 12000;

        public List<double> textCharactersID = new List<double>();
        public GameFont Font;
        public Sprite FontSprite;
        public string text { get; private set; }
        public int fontSize { get; private set; }
        public UIConstraint constraint { get; private set; }
        public Text() { }
        public Text(string _text, GameFont _font, Sprite _sprite, int _fontSize = 28, UIConstraint _constraint = UIConstraint.TopLeft)
        {
            Font = _font;
            FontSprite = _sprite;
            fontSize = _fontSize;
            constraint = _constraint;
            UpdateText(_text);
        }
        public void UpdateText(string _text)
        {
            textCharactersID.Clear();
            int textLength = 0;
            text = _text;
            for (int n = 0; n < _text.Length; n++)
            {
                char idx = _text[n];
                var character = Font.GetCharacter(idx);
                Loader.LoaderVertex data = Geometry.GetVertex(Geometries.Quad);
                RawModel charData;
                if (Font.characterRawModels.TryGetValue(character.id, out RawModel model)) charData = model;
                else
                {
                    data.textureCoords[0] = (float)character.x / Font.fontAtlas.Width;
                    data.textureCoords[1] = (float)character.y / Font.fontAtlas.Height;

                    data.textureCoords[2] = (float)character.x / Font.fontAtlas.Width;
                    data.textureCoords[3] = (float)(character.y + character.height) / Font.fontAtlas.Height;

                    data.textureCoords[4] = (float)(character.x + character.width) / Font.fontAtlas.Width;
                    data.textureCoords[5] = (float)(character.y + character.height) / Font.fontAtlas.Height;

                    data.textureCoords[6] = (float)(character.x + character.width) / Font.fontAtlas.Width;
                    data.textureCoords[7] = (float)character.y / Font.fontAtlas.Height;

                    data.position[4] = data.position[1] - (float)character.height * fontSize / ScalingConst;
                    data.position[6] = data.position[0] + (float)character.width * fontSize / ScalingConst;
                    data.position[7] = data.position[1] - (float)character.height * fontSize / ScalingConst;
                    data.position[9] = data.position[0] + (float)character.width * fontSize / ScalingConst;
                    charData = Loader.LoadToVAO(data);

                    Font.characterRawModels.Add(character.id, charData);
                }
                textCharactersID.Add(character.id);
                textLength += character.xadvance - 5;
            }
        }
    }
}
