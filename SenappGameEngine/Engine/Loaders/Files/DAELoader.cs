using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

using OpenTK;

using Senapp.Engine.Loaders.Abstractions;
using Senapp.Engine.Loaders.Models;
using static Senapp.Engine.Loaders.Models.ModelExtensions;

namespace Senapp.Engine.Loaders.Files
{
    public class DAELoader : IFileLoader<LoaderVertex>
    {
        public bool LoadFile(StringReader fileData, out LoaderVertex loaderVertex)
        {
            try
            {
                var data = fileData.ReadToEnd();
                fileData.Reset();

                var xmlReader = XmlReader.Create(fileData);
                var polyType = data.Contains("polylist") ? "polylist" : "triangles";

                xmlReader.ReadToFollowing("library_geometries");
                xmlReader.ReadToDescendant("mesh");
                xmlReader.ReadToDescendant("source");
                xmlReader.ReadToDescendant("float_array");
                xmlReader.Read();
                var verticesString = xmlReader.Value.Trim();

                xmlReader.ReadToFollowing("source");
                xmlReader.ReadToDescendant("float_array");
                xmlReader.Read();
                var normalString = xmlReader.Value.Trim();

                xmlReader.ReadToFollowing("source");
                xmlReader.ReadToDescendant("float_array");
                xmlReader.Read();
                var textureString = xmlReader.Value.Trim();

                xmlReader.ReadToFollowing(polyType);
                xmlReader.ReadToDescendant("p");
                xmlReader.Read();
                var trianglesString = xmlReader.Value.Trim();

                xmlReader.Close();

                List<ModelVertex> vertices = new();
                List<Vector2> textureCoords = new();
                List<Vector3> normals = new();
                List<int> indices = new();

                var verticesPoints = verticesString.Split(" ");
                for (int i = 0; i < verticesPoints.Length; i++)
                {
                    Vector3 vertex = new(
                        float.Parse(verticesPoints[0 + i], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(verticesPoints[1 + i], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(verticesPoints[2 + i], CultureInfo.InvariantCulture.NumberFormat));
                    vertices.Add(new ModelVertex(vertices.Count, vertex));
                    i += 2;
                }

                var texturePoints = textureString.Split(" ");
                for (int i = 0; i < texturePoints.Length; i++)
                {
                    Vector2 textureCoord = new(
                        float.Parse(texturePoints[0 + i], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(texturePoints[1 + i], CultureInfo.InvariantCulture.NumberFormat));
                    textureCoords.Add(textureCoord);
                    i += 1;
                }

                var normalPoints = normalString.Split(" ");
                for (int i = 0; i < normalPoints.Length; i++)
                {
                    Vector3 normal = new(
                        float.Parse(normalPoints[0 + i], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(normalPoints[1 + i], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(normalPoints[2 + i], CultureInfo.InvariantCulture.NumberFormat));
                    normals.Add(normal);
                    i += 2;
                }

                var trianglesPoints = trianglesString.Split(" ");
                for (int i = 0; i < trianglesPoints.Length; i++)
                {
                    int[] vertexData = new int[] {
                        int.Parse(trianglesPoints[0 + i], CultureInfo.InvariantCulture.NumberFormat),
                        int.Parse(trianglesPoints[2 + i], CultureInfo.InvariantCulture.NumberFormat),
                        int.Parse(trianglesPoints[1 + i], CultureInfo.InvariantCulture.NumberFormat)
                    };
                    ProcessVertex(vertexData, vertices, indices, 0);
                    i += 2;
                }

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

                if (vertices.Count == 0)
                {
                    throw new Exception("Error loading model file. File is either corrupt or not of type DAE.");
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