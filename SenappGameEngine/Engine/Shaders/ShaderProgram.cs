using System;
using System.Text;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

using Senapp.Engine.Utilities;

namespace Senapp.Engine.Shaders
{
    public abstract class ShaderProgram
    {
        public ShaderProgram(string vertexFileName, string fragmentFileName, bool fromResources)
        {
            this.fromResources = fromResources;

            programID = GL.CreateProgram();
            vertexShaderID = LoadShader(vertexFileName, ShaderType.VertexShader);
            fragmentShaderID = LoadShader(fragmentFileName, ShaderType.FragmentShader);

            GL.AttachShader(programID, vertexShaderID);
            GL.AttachShader(programID, fragmentShaderID);

            BindAttributes();
            GL.LinkProgram(programID);
            GL.ValidateProgram(programID);
            GetAllUniformLocations();
        }

        public void Start()
        {
            GL.UseProgram(programID);
        }
        public void Stop() 
        {
            GL.UseProgram(0);
        }
        public void Dispose()
        {
            Stop();
            GL.DetachShader(programID, vertexShaderID);
            GL.DetachShader(programID, fragmentShaderID);
            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);
            GL.DeleteProgram(programID);
        }

        #region Uniform Loading
        public void LoadInt(int location, int value)
        {
            GL.Uniform1(location, value);
        }
        public void LoadFloat(int location, float value)
        {
            GL.Uniform1(location, value);
        }
        public void LoadDouble(int location, double value)
        {
            GL.Uniform1(location, value);
        }
        public void LoadVector(int location, Vector2 value)
        {
            GL.Uniform2(location, value);
        }
        public void LoadVector(int location, Vector3 value)
        {
            GL.Uniform3(location, value);
        }
        public void LoadVector(int location, Vector4 value)
        {
            GL.Uniform4(location, value);
        }
        public void LoadBoolean(int location, bool value)
        {
            GL.Uniform1(location, value ? 1 : 0);
        }
        public void LoadMatrix(int location, Matrix4 value)
        {
            GL.UniformMatrix4(location, true, ref value);
        }
        #endregion
 
        protected int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(programID, uniformName);
        }
        protected void BindAttribute(int attribute, string variableName)
        {
            GL.BindAttribLocation(programID, attribute, variableName);
        }

        protected abstract void GetAllUniformLocations();
        protected abstract void BindAttributes();

        private int LoadShader(string fileName, ShaderType type)
        {
            StringBuilder shaderSource = new();
            try
            {
                var data = string.Empty;
                if (this.fromResources)
                {
                    data = Resources.GetFile(fileName);
                }
                else
                {
                    var lines = File.ReadAllLines(fileName);
                    StringBuilder sb = new StringBuilder();
                    foreach (var line in lines)
                    {
                        sb.AppendLine(line);
                    }
                    data = sb.ToString();
                }

                var reader = new StringReader(data);
                while(reader.Peek() != -1)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        shaderSource.AppendLine(line);
                    }
                }
                reader.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine($"[SHADER][ERROR] Could not read file - {e.Message}");
                Environment.Exit(1);
            }

            int shaderID = GL.CreateShader(type);

            GL.ShaderSource(shaderID, shaderSource.ToString());
            GL.CompileShader(shaderID);
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out int compileStatus);

            if (compileStatus == 0)
            {
                Console.WriteLine($"[SHADER][ERROR] Error compiling shader: Could not compile shader - {GL.GetShaderInfoLog(shaderID)}");
                Environment.Exit(1);
            }

            return shaderID;
        }

        private readonly int programID;
        private readonly int vertexShaderID;
        private readonly int fragmentShaderID;
        private readonly bool fromResources;
    }
}
