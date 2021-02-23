using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Senapp.Engine.Models
{
    public class Loader
    {
        private static List<int> vaos = new List<int>();
        private static List<int> vbos = new List<int>();
        private static List<int> textures = new List<int>();
        private static Dictionary<string, Texture> table = new Dictionary<string, Texture>();

        public static void DisposeModel(RawModel model)
        {
            foreach (var vbo in model.vboIDs)
            {
                GL.DeleteBuffer(vbo);
            }
            for (int i = 0; i < vaos.Count; i++)
                if (model.vaoID == vaos[i])
                {
                    GL.DeleteVertexArray(model.vaoID);
                    vaos.RemoveAt(i);
                    break;
                }
            for (int i = 0; i < vbos.Count; i++)
                for (int x = 0; x < model.vboIDs.Count; x++)
                    if (model.vboIDs[x] == vbos[i])
                    {
                        vbos.RemoveAt(i);
                        i--;
                    }
        }
        public static void DisposeTexture(Texture texture)
        {
            for (int i = 0; i < textures.Count; i++)
            {
                if (texture.GLTexture == textures[i])
                    textures.RemoveAt(i);
            }
            GL.DeleteTexture(texture.GLTexture);
        }
        public static void DisposeModelAndTexture(TexturedModel model)
        {
            DisposeModel(model.rawModel);
            DisposeTexture(model.texture);
        }
        public static RawModel LoadToVAO(LoaderVertex vertexData)
        {
            return LoadToVAO(vertexData.positions, vertexData.textureCoords, vertexData.normals, vertexData.indices);
        }
        public static RawModel LoadToVAO(float[] positions,float[] textureCoords, float[] normals, int[] indices)
        {
            int vaoID = CreateVAO();
            List<int> vboIDs = new List<int>();
            BindIndicesBuffer(indices);
            vboIDs.Add(FloatsToAttribute(0,3, positions));
            vboIDs.Add(FloatsToAttribute(1, 2, textureCoords));
            vboIDs.Add(FloatsToAttribute(2, 3, normals));

            UnbindVAO();
            return new RawModel(vaoID, indices.Length, vboIDs, new LoaderVertex(positions, textureCoords, normals, indices));
        }
        public static RawModel LoadToVAO(float[] positions, int dimensions)
        {
            int vaoID = CreateVAO();
            FloatsToAttribute(0, dimensions, positions);
            UnbindVAO();
            return new RawModel(vaoID, positions.Length / dimensions);
        }
        private static int CreateVAO()
        {
            int vaoID = GL.GenVertexArray();
            vaos.Add(vaoID);
            GL.BindVertexArray(vaoID);
            return vaoID;
        }
        public static int LoadCubeMap(string[] filePaths, string fileStart, string ext)
        {
            int texID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, texID);

            for (int i = 0; i < filePaths.Length; i++)
            {
                using (var image = new Bitmap("Resources/Textures/CubeMap/"+ fileStart + filePaths[i] + ext))
                {
                    var data = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);



                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                        0,
                        PixelInternalFormat.Rgba,
                        image.Width,
                        image.Height,
                        0,
                        PixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        data.Scan0);
                }
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            textures.Add(texID);
            return texID;
        }
        public static Texture LoadTexture(string filePath = null, string ext = ".png")
        {
            Texture tex;
            if (table.Count != 0)
            {
                if (filePath == null || filePath == "")  table.TryGetValue("DEFAULT_TEXTURE", out tex);
                else  table.TryGetValue(filePath, out tex);
            }
            else
            {
                Bitmap bitmap = new Bitmap("Engine/Defaults/DEFAULT_TEXTURE.png");
                tex = new Texture("DEFAULT_TEXTURE", bitmap, false, true);
                textures.Add(tex.GLTexture);
                table.Add("DEFAULT_TEXTURE", tex);
                tex = null;
            }
            if (tex == null)
            {
                Bitmap bitmap = null;
                try
                {
                    if (File.Exists("Resources/Textures/" + filePath + ext)) bitmap = new Bitmap("Resources/Textures/" + filePath + ext);
                    else bitmap = new Bitmap(filePath);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    if (table.Count != 0) table.TryGetValue("DEFAULT_TEXTURE", out tex);
                    if (tex != null) return tex;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    if (table.Count != 0)  table.TryGetValue("DEFAULT_TEXTURE", out tex);
                    if (tex != null) return tex;
                }

                tex = new Texture(filePath, bitmap, true, false);
                textures.Add(tex.GLTexture);
                
                if (filePath != null)
                    table.Add(filePath, tex);
            }
            return tex;
        }
        private static int FloatsToAttribute(int attributeNumber,int size, float[] data)
        {
            int vboID = GL.GenBuffer();
            vbos.Add(vboID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            byte[] buffer = FloatsToBytes(data);
            GL.BufferData(BufferTarget.ArrayBuffer, buffer.Length, buffer, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attributeNumber, size, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            return vboID;
        }
        private static void UnbindVAO()
        {
            GL.BindVertexArray(0);
        }
        private static void BindIndicesBuffer(int[] indices) 
        {
            int vboID = GL.GenBuffer();
            vbos.Add(vboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboID);
            byte[] buffer = IntsToBytes(indices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, buffer.Length, buffer, BufferUsageHint.StaticDraw);
        }
        private static byte[] IntsToBytes(int[] data)
        {
            var buffer = new byte[data.Length * Marshal.SizeOf(typeof(int))];
            System.Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        private static byte[] FloatsToBytes(float[] data)
        {
            var buffer = new byte[data.Length * Marshal.SizeOf(typeof(float))];
            System.Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
            return buffer;
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
    }
}
