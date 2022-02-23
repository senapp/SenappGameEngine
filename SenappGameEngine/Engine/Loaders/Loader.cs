using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

using Senapp.Engine.Utilities;
using Senapp.Engine.Models;

using Senapp.Engine.Loaders.Abstractions;
using Senapp.Engine.Loaders.Models;
using Senapp.Engine.Loaders.Files;
using Senapp.Engine.Renderer.Helper;
using static Senapp.Engine.Loaders.LoaderExtensions;

namespace Senapp.Engine.Loaders
{
    public class Loader
    {
        public static void Initialize()
        {
            modelLoaders.Add(ModelTypes.OBJ, new OBJLoader());
            modelLoaders.Add(ModelTypes.DAE, new DAELoader());
        }

        public static TexturedModel LoadGeometry(Geometries geometry, string texturePath, bool fromResources = true)
        {
            var texture = LoadTexture(texturePath, fromResources);
            return new TexturedModel(LoadToVAO(Geometry.GetVertex(geometry), Geometry.GetVertexName(geometry)), texture);
        }
        public static TexturedModel LoadModel(string filePath, ModelTypes type, string texturePath, bool fromResources = true)
        {
            Texture texture;
            if (string.IsNullOrEmpty(texturePath))
            {
                if (fromResources)
                {
                    texture = LoadTexture(filePath + "_tex", fromResources);
                }
                else
                {
                    texture = GetDefaultTexture();
                }
            }
            else
            {
                texture = LoadTexture(texturePath, fromResources);
            }

            if (!rawModelTable.TryGetValue(filePath, out var rawModel))
            {
                try
                {
                    StringReader stringReader;
                    LoaderVertex loaderVertex = null;
                    bool success = false;

                    if (fromResources)
                    {
                        stringReader = new StringReader(Resources.GetFile(filePath));
                    }
                    else
                    {
                        stringReader = new StringReader(File.ReadAllText(filePath));
                    }

                    if (modelLoaders.TryGetValue(type, out var loader)) 
                    {
                        success = loader.LoadFile(stringReader, out loaderVertex);
                    }
                    else
                    {
                        throw new Exception($"ModelType: {type} is not supported.");
                    }

                    stringReader.Close();

                    if (success)
                    {
                        return new TexturedModel(LoadToVAO(loaderVertex, filePath), texture);
                    }
                    else
                    {
                        throw new Exception($"Model with fileName: {filePath} and modelType: {type} could not be loaded.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[ENGINE][ERROR] {e.Message}");
                    return new TexturedModel(LoadToVAO(Geometry.GetVertex(Geometries.Cube), Geometry.GetVertexName(Geometries.Cube)), texture);
                }
            } 
            else
            {
                return new TexturedModel(rawModel, texture);
            }
        }

        public static RawModel LoadToVAO(LoaderVertex vertexData, string vertexName)
        {
            if (rawModelTable.TryGetValue(vertexName, out var rawModel))
            {
                return rawModel;
            }

            int vaoID = CreateVAO();
            vaos.Add(vaoID);

            var vboIDs = new List<int>
            {
                BindIndicesBuffer(vertexData.indices),
                FloatsToAttribute(0, 3, vertexData.positions),
                FloatsToAttribute(1, 2, vertexData.textureCoords),
                FloatsToAttribute(2, 3, vertexData.normals)
            };
            vbos.AddRange(vboIDs);

            UnbindVAO();

            var model = new RawModel(vaoID, vertexData.indices.Length, vboIDs, vertexData, vertexName);
            rawModelTable.Add(vertexName, model);

            return model;
        }
        public static RawModel LoadPositionsToVAO(float[] positions, int dimensions, string modelName)
        {
            if (rawModelTable.TryGetValue(modelName, out var rawModel))
            {
                return rawModel;
            }

            int vaoID = CreateVAO();

            vaos.Add(vaoID);
            vbos.Add(FloatsToAttribute(0, dimensions, positions));

            UnbindVAO();

            var model = new RawModel(vaoID, positions.Length / dimensions);
            rawModelTable.Add(modelName, model);
            return model;
        }

        public static Texture LoadTexture(string filePath = "", bool fromResources = true)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return GetDefaultTexture();
            }
            else
            {
                if (!textureTable.TryGetValue(filePath, out var tex))
                {
                    Bitmap bitmap;
                    try
                    {
                        if (fromResources)
                        {
                            bitmap = Resources.GetImage(filePath);
                            if (bitmap == null) throw new Exception($"Could not load Bitmap from resources. Name: {filePath}");
                        }
                        else
                        {
                            bitmap = new Bitmap(filePath);
                            if (bitmap == null) throw new Exception($"Could not load Bitmap from path. Path: {filePath}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[ENGINE][ERROR] {e.Message}");
                        return GetDefaultTexture();
                    }

                    tex = new Texture(filePath, bitmap, true, false);
                    textures.Add(tex.GLTexture);
                    textureTable.Add(filePath, tex);
                    return tex;
                }
                return tex;
            }
        }
        public static int LoadCubeMap(string[] filePaths, string fileStart, bool fromResources = true)
        {
            int texID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, texID);

            for (int i = 0; i < filePaths.Length; i++)
            {
                Bitmap bitmap;
                try
                {
                    if (fromResources)
                    {
                        bitmap = Resources.GetImage($"{fileStart}{filePaths[i]}");
                        if (bitmap == null) throw new Exception($"[ENGINE][ERROR] Could not load Bitmap from resources. Name: {fileStart}{filePaths[i]}");
                    }
                    else
                    {
                        bitmap = Resources.GetImage($"{fileStart}{filePaths[i]}");
                        if (bitmap == null) throw new Exception($"[ENGINE][ERROR] Could not load Bitmap from path. Path: {fileStart}{filePaths[i]}");
                    }
                }
                catch 
                {
                    bitmap = Resources.GetImage(defaultTexture);
                }

                var data = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                    0,
                    PixelInternalFormat.Rgba,
                    bitmap.Width,
                    bitmap.Height,
                    0,
                    PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    data.Scan0);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            textures.Add(texID);
            return texID;
        }

        public static void DisposeModel(RawModel model)
        {
            foreach (var vbo in model.VboIds)
                GL.DeleteBuffer(vbo);

            for (int i = 0; i < vaos.Count; i++)
                if (model.VaoId == vaos[i])
                {
                    GL.DeleteVertexArray(model.VaoId);
                    vaos.RemoveAt(i);
                    break;
                }

            for (int i = 0; i < vbos.Count; i++)
                for (int x = 0; x < model.VboIds.Count; x++)
                    if (model.VboIds[x] == vbos[i])
                    {
                        vbos.RemoveAt(i);
                        i--;
                    }
        }
        public static void DisposeTexture(Texture texture)
        {
            for (int i = 0; i < textures.Count; i++)
                if (texture.GLTexture == textures[i])
                    textures.RemoveAt(i);

            GL.DeleteTexture(texture.GLTexture);
        }
        public static void Dispose()
        {
            foreach(int vao in vaos)
                GL.DeleteVertexArray(vao);

            foreach (int vbo in vbos)
                GL.DeleteBuffer(vbo);

            foreach (int texture in textures)
                GL.DeleteTexture(texture);
        }

        private static Texture GetDefaultTexture()
        {
            if (!textureTable.TryGetValue(defaultTexture, out var tex))
            {
                Bitmap bitmap = Resources.GetImage(defaultTexture);
                tex = new Texture(defaultTexture, bitmap, false, true);
                textures.Add(tex.GLTexture);
                textureTable.Add(defaultTexture, tex);
            }
            return tex;
        }
        private const string defaultTexture = "default_texture";

        private static readonly Dictionary<string, Texture> textureTable = new();
        private static readonly Dictionary<string, RawModel> rawModelTable = new();
        private static readonly Dictionary<ModelTypes, IFileLoader<LoaderVertex>> modelLoaders = new();

        private static readonly List<int> vaos = new();
        private static readonly List<int> vbos = new();
        private static readonly List<int> textures = new();
    }
}
