using System;
using System.Collections.Generic;
using System.IO;

using OpenTK;
using Senapp.Engine.Loaders;
using Senapp.Engine.Models;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.UI
{
    public class GameFont
    {
        // Use bmfont1.14a to get .fnt and .png files, preferable with transparent background and white characthers

        public string face;
        public int size;
        public bool bold;
        public bool italic;
        public string charset;
        public bool unicode;
        public int stretchH;
        public bool smooth;
        public bool aa;
        public Vector4 padding;
        public Vector2 spacing;
        public int lineHeight;
        public int fontBase;
        public int scaleW;
        public int scaleH;
        public int pages;
        public bool packed;
        public int pageID;
        public string file;
        public Texture fontAtlas;

        public Dictionary<double, RawModel> characterRawModels = new();

        public List<Character> characters = new();
        public Character GetCharacter(double id)
        {
            foreach (var item in characters)
            {
                if (item.id == id) return item;
            }
            return null;
        }
        public void LoadFont(string fontName)
        {
            characters.Clear();
            var sr = new StringReader(Resources.GetFile(fontName));
            while (sr.Peek() != -1)
            {
                string line = sr.ReadLine();
                if (line.StartsWith("char id"))
                {
                    characters.Add(new Character(
                        GetDouble("id=", line),
                        GetInt("x=", line),
                        GetInt("y=", line),
                        GetInt("width=", line),
                        GetInt("height=", line),
                        GetInt("xoffset=", line),
                        GetInt("yoffset=", line),
                        GetInt("xadvance=", line),
                        GetInt("page=", line),
                        GetInt("chnl=", line)
                     ));
                }
                else
                {
                    if (line.Contains("face=")) face = GetString("face=", line);
                    if (line.Contains("size=")) size = GetInt("size=", line);
                    if (line.Contains("bold=")) bold = GetBool("bold=", line);
                    if (line.Contains("italic=")) italic = GetBool("italic=", line);
                    if (line.Contains("charset=")) charset = GetString("charset=", line);
                    if (line.Contains("unicode=")) unicode = GetBool("unicode=", line);
                    if (line.Contains("stretchH=")) stretchH = GetInt("stretchH=", line);
                    if (line.Contains("smooth=")) smooth = GetBool("smooth=", line);
                    if (line.Contains("aa=")) aa = GetBool("aa=", line);
                    if (line.Contains("padding="))
                    {
                        var strings = GetString("padding=", line).Split(",");
                        int[] numbers = new int[strings.Length];
                        for (int i = 0; i < strings.Length; i++)
                        {
                            numbers[i] = Convert.ToInt32(strings[i]);
                        }
                        padding = new Vector4(numbers[0], numbers[1], numbers[2], numbers[3]);
                    }
                    if (line.Contains("spacing="))
                    {
                        var strings = GetString("spacing=", line).Split(",");
                        int[] numbers = new int[strings.Length];
                        for (int i = 0; i < strings.Length; i++)
                        {
                            numbers[i] = Convert.ToInt32(strings[i]);
                        }
                        spacing = new Vector2(numbers[0], numbers[1]);
                    }
                    if (line.Contains("lineHeight=")) lineHeight = GetInt("lineHeight=", line);
                    if (line.Contains("base=")) fontBase = GetInt("base=", line);
                    if (line.Contains("scaleW=")) scaleW = GetInt("scaleW=", line);
                    if (line.Contains("scaleH=")) scaleH = GetInt("scaleH=", line);
                    if (line.Contains("pages=")) pages = GetInt("pages=", line);
                    if (line.Contains("packed=")) packed = GetBool("packed=", line);
                    if (line.Contains("page id=")) pageID = GetInt("page id=", line);
                    // if (line.Contains("file=")) file = GetString("file=", line);
                }
            }
            sr.Close();
            fontAtlas = Loader.LoadTexture($"{fontName}_img");
        }

        private string GetString(string identifier, string text)
        {
            int dynVal = 0;
            if (text.IndexOf(" ", text.IndexOf(identifier)) < text.IndexOf(identifier))
                dynVal = text.Length + 1;
            return text.Substring(text.IndexOf(identifier) + identifier.Length, (dynVal + text.IndexOf(" ", text.IndexOf(identifier) + identifier.Length)) - (text.IndexOf(identifier) + identifier.Length)).Replace("\"", "").Trim();
        }
        private int GetInt(string identifier, string text)
        {
            int dynVal = 0;
            if (text.IndexOf(" ", text.IndexOf(identifier)) < text.IndexOf(identifier))
                dynVal = text.Length + 1;
            return Convert.ToInt32(text.Substring(text.IndexOf(identifier) + identifier.Length, (dynVal + text.IndexOf(" ", text.IndexOf(identifier) + identifier.Length)) - (text.IndexOf(identifier) + identifier.Length)));
        }
        private double GetDouble(string identifier, string text)
        {
            int dynVal = 0;
            if (text.IndexOf(" ", text.IndexOf(identifier)) < text.IndexOf(identifier))
                dynVal = text.Length + 1;
            return double.Parse(text.Substring(text.IndexOf(identifier) + identifier.Length, (dynVal + text.IndexOf(" ", text.IndexOf(identifier) + identifier.Length)) - (text.IndexOf(identifier) + identifier.Length)));
        }
        private bool GetBool(string identifier, string text)
        {
            int dynVal = 0;
            if (text.IndexOf(" ", text.IndexOf(identifier)) < text.IndexOf(identifier))
                dynVal = text.Length + 1;
            int a = Convert.ToInt32(text.Substring(text.IndexOf(identifier) + identifier.Length,(dynVal+ text.IndexOf(" ", text.IndexOf(identifier) + identifier.Length)) - (text.IndexOf(identifier) + identifier.Length)));
            if (a == 0) return false;
            else return true;
        }
    }

    public class Character
    {
        public double id;
        public int x;
        public int y;
        public int width;
        public int height;
        public int xoffset;
        public int yoffset;
        public int xadvance;
        public int page;
        public int chnl;

        public Character(double _id, int _x, int _y, int _width, int _height, int _xoffset, int _yoffset, int _xadvance, int _page, int _chnl)
        {
            id = _id;
            x = _x;
            y = _y;
            width = _width;
            height = _height;
            xoffset = _xoffset;
            yoffset = _yoffset;
            xadvance = _xadvance;
            page = _page;
            chnl = _chnl;
        }
    }
}
