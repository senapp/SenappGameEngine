using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;

namespace Senapp.Engine.Loaders
{
    public static class LoaderExtensions
    {
        public static byte[] IntsToBytes(int[] data)
        {
            var buffer = new byte[data.Length * Marshal.SizeOf(typeof(int))];
            Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        public static byte[] FloatsToBytes(float[] data)
        {
            var buffer = new byte[data.Length * Marshal.SizeOf(typeof(float))];
            Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
            return buffer;
        }

        public static int FloatsToAttribute(int attributeNumber, int size, float[] data)
        {
            int vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            byte[] buffer = FloatsToBytes(data);
            GL.BufferData(BufferTarget.ArrayBuffer, buffer.Length, buffer, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attributeNumber, size, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            return vboID;
        }

        public static int BindIndicesBuffer(int[] indices)
        {
            int vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboID);

            byte[] buffer = IntsToBytes(indices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, buffer.Length, buffer, BufferUsageHint.StaticDraw);
            return vboID;
        }

        public static int CreateVAO()
        {
            int vaoID = GL.GenVertexArray();
            GL.BindVertexArray(vaoID);
            return vaoID;
        }

        public static void UnbindVAO()
        {
            GL.BindVertexArray(0);
        }

        public static void Reset(this StringReader reader)
        {
            reader.GetType()
                  .GetField("_pos", BindingFlags.NonPublic | BindingFlags.Instance)
                  .SetValue(reader, 0);
        }
    }
}
