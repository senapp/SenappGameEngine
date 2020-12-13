using OpenTK;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
        private static Dictionary<string, Loader.LoaderVertex> table = new Dictionary<string, Loader.LoaderVertex>();

        public static Loader.LoaderVertex LoadOBJModel(string fileName, float scaleFactor = 1, int verticeDecimals = 100, bool storeData = false)
        {
            StreamReader reader = null;
            Loader.LoaderVertex model = null;
            if (table.Count != 0)
            {
                if (fileName == null)
                    table.TryGetValue("DEFAULT_MODEL", out model);
                else
                    table.TryGetValue(fileName, out model);
            }
           if (model == null)
            {
                try
                {
                    if (fileName == null)
                    {
                        table.Add("DEFAULT_MODEL", Geometry.GetVertex(Geometries.Cube));
                        return Geometry.GetVertex(Geometries.Cube);
                    }
                    else
                        reader = new StreamReader("Resources/Models/" + fileName + ".obj");
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
                string line = "Start";
                int count = 0;
                bool isSeperated = false;
                while (line != null)
                {
                    if (line.StartsWith("o "))
                        count++;
                    line = reader.ReadLine();
                }
                if (count > 1)
                    isSeperated = true;
                line = "Start";
                reader.DiscardBufferedData();
                reader.BaseStream.Position = 0;
                List<Vertex> vertices = new List<Vertex>();
                List<Vector2> textureCoords = new List<Vector2>();
                List<Vector3> normals = new List<Vector3>();
                List<int> indices = new List<int>();

                {
                    if (!isSeperated)
                    {
                        while (line != null)
                        {
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
                            else if (line.StartsWith("f "))
                            {
                                break;
                            }
                            line = reader.ReadLine();
                        }
                        while (line != null)
                        {
                            if (!line.StartsWith("f "))
                            {
                                line = reader.ReadLine();
                                continue;
                            }
                            string[] currentLine = line.Split(" ");
                            string[] vertex1 = currentLine[1].Split("/");
                            string[] vertex2 = currentLine[2].Split("/");
                            string[] vertex3 = currentLine[3].Split("/");
                            ProcessVertex(vertex1, vertices, indices);
                            ProcessVertex(vertex2, vertices, indices);
                            ProcessVertex(vertex3, vertices, indices);

                            line = reader.ReadLine();

                        }
                        reader.Close();
                    }
                    else
                    {
                        while (line != null)
                        {
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
                            line = reader.ReadLine();
                        }
                        line = "Start";
                        reader.DiscardBufferedData();
                        reader.BaseStream.Position = 0;
                        while (line != null)
                        {
                            if (!line.StartsWith("f "))
                            {
                                line = reader.ReadLine();
                                continue;
                            }
                            string[] currentLine = line.Split(" ");
                            string[] vertex1 = currentLine[1].Split("/");
                            string[] vertex2 = currentLine[2].Split("/");
                            string[] vertex3 = currentLine[3].Split("/");
                            ProcessVertex(vertex1, vertices, indices);
                            ProcessVertex(vertex2, vertices, indices);
                            ProcessVertex(vertex3, vertices, indices);

                            line = reader.ReadLine();

                        }
                        reader.Close();
                    }
                }

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

                if (scaleFactor != 1)
                    for (int i = 0; i < verticesArray.Length; i++)
                    {
                        verticesArray[i] /= scaleFactor;
                        verticesArray[i] = (float)Math.Round(verticesArray[i] * (float)verticeDecimals) / (float)verticeDecimals;
                    }

                if (storeData)
                {
                    string writeText = null;
                    for (int i = 0; i < verticesArray.Length; i += 3)
                        writeText += ("\n" + verticesArray[i] + "f?" + verticesArray[i + 1] + "f?" + verticesArray[i + 2] + "f?");

                    writeText += ("\n\n\n");
                    for (int i = 0; i < textureCoordsArray.Length; i += 2)
                        writeText += ("\n" + textureCoordsArray[i] + "f?" + textureCoordsArray[i + 1] + "f?");

                    writeText += ("\n\n\n");
                    for (int i = 0; i < normalsArray.Length; i += 3)
                        writeText += ("\n" + normalsArray[i] + "f?" + normalsArray[i + 1] + "f?" + normalsArray[i + 2] + "f?");

                    writeText += ("\n\n\n");
                    for (int i = 0; i < indicesArray.Length; i += 3)
                        writeText += ("\n" + indicesArray[i] + "?" + indicesArray[i + 1] + "?" + indicesArray[i + 2] + "?");
                    writeText = writeText.Replace(",", ".");
                    writeText = writeText.Replace("?", ",");

                    File.WriteAllText("Resources/Models/GeometryData.txt", writeText);
                }
                Loader.LoaderVertex data = new Loader.LoaderVertex(verticesArray, textureCoordsArray, normalsArray, indicesArray, furthest);
                table.Add(fileName, data);
                return data;
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
                    Vertex duplicateVertex = new Vertex(vertices.Count, previousVertex.position) ;
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
