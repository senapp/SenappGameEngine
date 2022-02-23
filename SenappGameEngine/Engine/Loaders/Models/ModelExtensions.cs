using System;
using System.Collections.Generic;

using OpenTK;

namespace Senapp.Engine.Loaders.Models
{
    public static class ModelExtensions
    {
        public static void ProcessVertex(int[] vertexData, List<ModelVertex> vertices, List<int> indices, int offset = 1)
        {
            var index = vertexData[0] - offset;
            var currentVertex = vertices[index];
            var textureIndex = vertexData[1] - offset;
            var normalIndex = vertexData[2] - offset;

            if (!currentVertex.IsSet())
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
        public static float ConvertDataToArrays(List<ModelVertex> vertices, List<Vector2> textures, List<Vector3> normals, float[] verticesArray, float[] texturesArray, float[] normalsArray)
        {
            var furthestPoint = 0f;

            for (int i = 0; i < vertices.Count; i++)
            {
                var currentVertex = vertices[i];
                if (currentVertex.length > furthestPoint)
                {
                    furthestPoint = currentVertex.length;
                }

                var position = currentVertex.position;
                var textureCoord = textures[currentVertex.textureIndex];
                var normalVector = normals[currentVertex.normalIndex];

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
        public static void DealWithAlreadyProcessedVertex(ModelVertex previousVertex, int newTextureIndex, int newNormalIndex, List<int> indices, List<ModelVertex> vertices)
        {
            if (previousVertex.HasSameTextureAndNormal(newTextureIndex, newNormalIndex))
            {
                indices.Add(previousVertex.index);
            }
            else
            {
                if (previousVertex.duplicateVertex != null)
                {
                    DealWithAlreadyProcessedVertex(previousVertex.duplicateVertex, newTextureIndex, newNormalIndex, indices, vertices);
                }
                else
                {
                    var duplicateVertex = new ModelVertex(vertices.Count, previousVertex.position);

                    duplicateVertex.textureIndex = newTextureIndex;
                    duplicateVertex.normalIndex = newNormalIndex;
                    previousVertex.duplicateVertex = duplicateVertex;

                    vertices.Add(duplicateVertex);
                    indices.Add(duplicateVertex.index);
                }
            }
        }
        public static void RemoveUnusedVertices(List<ModelVertex> vertices)
        {
            foreach (ModelVertex vertex in vertices)
            {
                if (!vertex.IsSet())
                {
                    vertex.textureIndex = 0;
                    vertex.normalIndex = 0;
                }
            }
        }
    } 
}
