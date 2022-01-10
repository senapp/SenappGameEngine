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
        private int programID;
        private int vertexShaderID;
        private int fragmentShaderID;


        public ShaderProgram(string vertexFileName, string fragmentFileName)
        {
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
        protected abstract void GetAllUniformLocations();
 
        protected int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(programID, uniformName);
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
        public void Start()
        {
            GL.UseProgram(programID);
        }
        public void Stop() 
        {
            GL.UseProgram(0);
        }
        public void CleanUp()
        {
            Stop();
            GL.DetachShader(programID, vertexShaderID);
            GL.DetachShader(programID, fragmentShaderID);
            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);
            GL.DeleteProgram(programID);
        }
        protected abstract void BindAttributes();

        protected void BindAttribute(int attribute, string variableName)
        {
            GL.BindAttribLocation(programID, attribute, variableName);
        }
        private static int LoadShader(string fileName, ShaderType type)
        {
            StringBuilder shaderSource = new StringBuilder();
            try
            {
                var reader = new StringReader(Resources.GetFile(fileName));
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
                Console.WriteLine("Could not read file");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            int shaderID = GL.CreateShader(type);
            GL.ShaderSource(shaderID, shaderSource.ToString());
            GL.CompileShader(shaderID);
            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out int compileStatus);
            if (compileStatus == 0)
            {
                Console.WriteLine("Error compiling shader: Could not compile shader");
                Console.WriteLine(GL.GetShaderInfoLog(shaderID));
                Environment.Exit(1);
            }
            return shaderID;
        }
    }
}
