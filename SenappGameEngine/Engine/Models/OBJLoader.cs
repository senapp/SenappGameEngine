using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using OpenTK;

using Senapp.Engine.Renderer;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Models
{
    public class Vertex
    {
        private static readonly int NO_INDEX = -1;

        public Vector3 position;
        public int textureIndex = NO_INDEX;
        public int normalIndex = NO_INDEX;
        public Vertex duplicateVertex = null;
        public int index;
        public float length;


        public Vertex(int index, Vector3 position)
        {
            this.index = index;
            this.position = position;
            this.length = position.Length;
        }

        public bool isSet()
        {
            return textureIndex != NO_INDEX && normalIndex != NO_INDEX;
        }

        public bool hasSameTextureAndNormal(int textureIndexOther, int normalIndexOther)
        {
            return textureIndexOther == textureIndex && normalIndexOther == normalIndex;
        }
    }
    public class OBJLoader
    {
        private static Dictionary<string, LoaderVertex> table = new Dictionary<string, LoaderVertex>();
        public static LoaderVertex LoadOBJModel(string fileName)
        {
            LoaderVertex model = null;
            if (table.Count != 0)
            {
                if (fileName == null) table.TryGetValue("default_model", out model);
                else table.TryGetValue(fileName, out model);
            }
            if (model == null)
            {
                try
                {
                    if (fileName == null)
                    {
                        table.Add("default_model", Geometry.GetVertex(Geometries.Cube));
                        return Geometry.GetVertex(Geometries.Cube);
                    }

                    var reader = new StringReader(Resources.GetFile(fileName));

                    var line = "Start";
                    List<Vertex> vertices = new List<Vertex>();
                    List<Vector2> textureCoords = new List<Vector2>();
                    List<Vector3> normals = new List<Vector3>();
                    List<int> indices = new List<int>();

                    while (reader.Peek() != -1)
                    {
                        if (line != null && line.Length != 0)
                        {
                            if (line.Contains("  ")) line = line.Replace("  ", " ");
                            string[] currentLine = line.Split(" ");
                            if (line.StartsWith("v "))
                            {
                                Vector3 vertex = new Vector3(float.Parse(currentLine[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(currentLine[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(currentLine[3], CultureInfo.InvariantCulture.NumberFormat));
                                Vertex newVertex = new Vertex(vertices.Count, vertex);
                                vertices.Add(newVertex);
                            }
                            else if (line.StartsWith("vt "))
                            {
                                Vector2 textureCoord = new Vector2(float.Parse(currentLine[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(currentLine[2], CultureInfo.InvariantCulture.NumberFormat));
                                textureCoords.Add(textureCoord);
                            }
                            else if (line.StartsWith("vn "))
                            {
                                Vector3 normal = new Vector3(float.Parse(currentLine[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(currentLine[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(currentLine[3], CultureInfo.InvariantCulture.NumberFormat));
                                normals.Add(normal);
                            }
                        }
                        line = reader.ReadLine();
                    }

                    reader = new StringReader(Resources.GetFile(fileName));

                    while (reader.Peek() != -1)
                    {
                        if (line != null && line.Length != 0)
                        {
                            if (line.Contains("  ")) line = line.Replace("  ", " ");
                            if (line.StartsWith("f "))
                            {
                                string[] currentLine = line.Split(" ");
                                string[] vertex1 = currentLine[1].Split("/");
                                string[] vertex2 = currentLine[2].Split("/");
                                string[] vertex3 = currentLine[3].Split("/");
                                ProcessVertex(vertex1, vertices, indices);
                                ProcessVertex(vertex2, vertices, indices);
                                ProcessVertex(vertex3, vertices, indices);
                            }
                        }
                        line = reader.ReadLine();
                    }
                    reader.Close();
                    RemoveUnusedVertices(vertices);
                    float[] verticesArray = new float[vertices.Count * 3];
                    float[] textureCoordsArray = new float[vertices.Count * 2];
                    float[] normalsArray = new float[vertices.Count * 3];
                    float furthest = ConvertDataToArrays(vertices, textureCoords, normals, verticesArray, textureCoordsArray, normalsArray);
                    int[] indicesArray = ConvertIndicesListToArray(indices);

                    int vertexPointer = 0;
                    foreach (Vertex vertex in vertices)
                    {
                        verticesArray[vertexPointer++] = vertex.position.X;
                        verticesArray[vertexPointer++] = vertex.position.Y;
                        verticesArray[vertexPointer++] = vertex.position.Z;
                    }
                    for (int i = 0; i < indices.Count; i++)
                        indicesArray[i] = indices[i];

                    LoaderVertex data = new LoaderVertex(verticesArray, textureCoordsArray, normalsArray, indicesArray, furthest);
                    table.Add(fileName, data);
                    return data;
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    return Geometry.GetVertex(Geometries.Cube);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return Geometry.GetVertex(Geometries.Cube);
                }
            }
            else
            {
                return model;
            }

        }

        public static void ProcessVertex(string[] vertex, List<Vertex> vertices, List<int> indices)
        {
            int index = int.Parse(vertex[0]) - 1;
            Vertex currentVertex = vertices[index];
            int textureIndex = int.Parse(vertex[1]) - 1;
            int normalIndex = int.Parse(vertex[2]) - 1;
            if (!currentVertex.isSet())
            {
                currentVertex.textureIndex = textureIndex;
                currentVertex.normalIndex = normalIndex;
                indices.Add(index);
            }
            else
            {
                DealWithAlreadyProcessedVertex(currentVertex, textureIndex, normalIndex, indices, vertices);
            }
        }
        private static int[] ConvertIndicesListToArray(List<int> indices)
        {
            int[] indicesArray = new int[indices.Count];
            for (int i = 0; i < indicesArray.Length; i++)
            {
                indicesArray[i] = indices[i];
            }
            return indicesArray;
        }

        private static float ConvertDataToArrays(List<Vertex> vertices, List<Vector2> textures,
                List<Vector3> normals, float[] verticesArray, float[] texturesArray,
                float[] normalsArray)
        {
            float furthestPoint = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                Vertex currentVertex = vertices[i];
                if (currentVertex.length > furthestPoint)
                {
                    furthestPoint = currentVertex.length;
                }
                Vector3 position = currentVertex.position;
                Vector2 textureCoord = textures[currentVertex.textureIndex];
                Vector3 normalVector = normals[currentVertex.normalIndex];
                verticesArray[i * 3] = position.X;
                verticesArray[i * 3 + 1] = position.Y;
                verticesArray[i * 3 + 2] = position.Z;
                texturesArray[i * 2] = textureCoord.X;
                texturesArray[i * 2 + 1] = 1 - textureCoord.Y;
                normalsArray[i * 3] = normalVector.X;
                normalsArray[i * 3 + 1] = normalVector.Y;
                normalsArray[i * 3 + 2] = normalVector.Z;
            }
            return furthestPoint;
        }

        private static void DealWithAlreadyProcessedVertex(Vertex previousVertex, int newTextureIndex, int newNormalIndex, List<int> indices, List<Vertex> vertices)
        {
            if (previousVertex.hasSameTextureAndNormal(newTextureIndex, newNormalIndex))
            {
                indices.Add(previousVertex.index);
            }
            else
            {
                Vertex anotherVertex = previousVertex.duplicateVertex;
                if (anotherVertex != null)
                {
                    DealWithAlreadyProcessedVertex(anotherVertex, newTextureIndex, newNormalIndex, indices, vertices);
                }
                else
                {
                    Vertex duplicateVertex = new Vertex(vertices.Count, previousVertex.position);
                    duplicateVertex.textureIndex = newTextureIndex;
                    duplicateVertex.normalIndex = newNormalIndex;
                    previousVertex.duplicateVertex = duplicateVertex;
                    vertices.Add(duplicateVertex);
                    indices.Add(duplicateVertex.index);
                }

            }
        }

        private static void RemoveUnusedVertices(List<Vertex> vertices)
        {
            foreach (Vertex vertex in vertices)
            {
                if (!vertex.isSet())
                {
                    vertex.textureIndex = 0;
                    vertex.normalIndex = 0;
                }
            }
        }
    }
}