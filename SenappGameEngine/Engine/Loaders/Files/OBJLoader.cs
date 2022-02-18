using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using OpenTK;

using Senapp.Engine.Loaders.Abstractions;
using Senapp.Engine.Loaders.Models;
using static Senapp.Engine.Loaders.Models.ModelExtensions;

namespace Senapp.Engine.Loaders.Files
{
    public class OBJLoader : IFileLoader<LoaderVertex>
    {
        public bool LoadFile(StringReader fileData, out LoaderVertex loaderVertex)
        {
            try
            {
                var line = "Start";
                List<ModelVertex> vertices = new();
                List<Vector2> textureCoords = new();
                List<Vector3> normals = new();
                List<int> indices = new();

                while (fileData.Peek() != -1)
                {
                    if (line != null && line.Length != 0)
                    {
                        if (line.Contains("  ")) line = line.Replace("  ", " ");
                        string[] currentLine = line.Split(" ");
                        if (line.StartsWith("v "))
                        {
                            Vector3 vertex = new(float.Parse(currentLine[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(currentLine[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(currentLine[3], CultureInfo.InvariantCulture.NumberFormat));
                            ModelVertex newVertex = new(vertices.Count, vertex);
                            vertices.Add(newVertex);
                        }
                        else if (line.StartsWith("vt "))
                        {
                            Vector2 textureCoord = new(float.Parse(currentLine[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(currentLine[2], CultureInfo.InvariantCulture.NumberFormat));
                            textureCoords.Add(textureCoord);
                        }
                        else if (line.StartsWith("vn "))
                        {
                            Vector3 normal = new(float.Parse(currentLine[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(currentLine[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(currentLine[3], CultureInfo.InvariantCulture.NumberFormat));
                            normals.Add(normal);
                        }
                    }
                    line = fileData.ReadLine();
                }

                fileData.Reset();

                while (fileData.Peek() != -1)
                {
                    if (line != null && line.Length != 0)
                    {
                        if (line.Contains("  ")) line = line.Replace("  ", " ");
                        if (line.StartsWith("f "))
                        {
                            string[] currentLine = line.Split(" ");
                            int[] vertex1 = currentLine[1].Split("/").Select(value => int.Parse(value, CultureInfo.InvariantCulture.NumberFormat)).ToArray();
                            int[] vertex2 = currentLine[2].Split("/").Select(value => int.Parse(value, CultureInfo.InvariantCulture.NumberFormat)).ToArray();
                            int[] vertex3 = currentLine[3].Split("/").Select(value => int.Parse(value, CultureInfo.InvariantCulture.NumberFormat)).ToArray();
                            ProcessVertex(vertex1, vertices, indices);
                            ProcessVertex(vertex2, vertices, indices);
                            ProcessVertex(vertex3, vertices, indices);
                        }
                    }
                    line = fileData.ReadLine();
                }

                fileData.Close();

                RemoveUnusedVertices(vertices);
                float[] verticesArray = new float[vertices.Count * 3];
                float[] textureCoordsArray = new float[vertices.Count * 2];
                float[] normalsArray = new float[vertices.Count * 3];
                float furthest = ConvertDataToArrays(vertices, textureCoords, normals, verticesArray, textureCoordsArray, normalsArray);
                int[] indicesArray = indices.ToArray();

                int vertexPointer = 0;
                foreach (ModelVertex vertex in vertices)
                {
                    verticesArray[vertexPointer++] = vertex.position.X;
                    verticesArray[vertexPointer++] = vertex.position.Y;
                    verticesArray[vertexPointer++] = vertex.position.Z;
                }
                for (int i = 0; i < indices.Count; i++)
                    indicesArray[i] = indices[i];

                if (verticesArray.Length == 0)
                {
                    throw new Exception("Error loading model file. File is either corrupt or not of type OBJ.");
                }

                loaderVertex = new LoaderVertex(verticesArray, textureCoordsArray, normalsArray, indicesArray);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ENGINE][ERROR] {e.Message}");
                loaderVertex = null;
                return false;
            }
        }
    }
}