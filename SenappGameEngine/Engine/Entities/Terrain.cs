
using Senapp.Engine.Models;
using Senapp.Engine.Loaders;
using Senapp.Engine.Core.Components;
using System;

namespace Senapp.Engine.Entities
{
    public class Terrain
    {


        public static RawModel GenerateTerrain(float size = 10, int vertex_count = 9, int tiling = 1)
        {
            vertex_count = vertex_count * tiling - (tiling - 1);

            int count = vertex_count * vertex_count;
            float[] vertices = new float[count * 3];
            float[] normals = new float[count * 3];
            float[] textureCoords = new float[count * 2];
            int[] indices = new int[6 * (vertex_count - 1) * (vertex_count - 1)];
            int vertexPointer = 0;
            for (int i = 0; i < vertex_count; i++)
            {
                for (int j = 0; j < vertex_count; j++)
                {
                    vertices[vertexPointer * 3] = (float)j / ((float)vertex_count - 1) * size;
                    vertices[vertexPointer * 3 + 1] = 0;
                    vertices[vertexPointer * 3 + 2] = (float)i / ((float)vertex_count - 1) * size;
                    normals[vertexPointer * 3] = 0;
                    normals[vertexPointer * 3 + 1] = 1;
                    normals[vertexPointer * 3 + 2] = 0;
                    textureCoords[vertexPointer * 2] = (float)j / ((float)vertex_count - 1) * tiling;
                    textureCoords[vertexPointer * 2 + 1] = (float)i / ((float)vertex_count - 1) * tiling;
                    vertexPointer++;
                }
            }
            int pointer = 0;
            for (int gz = 0; gz < vertex_count - 1; gz++)
            {
                for (int gx = 0; gx < vertex_count - 1; gx++)
                {
                    int topLeft = (gz * vertex_count) + gx;
                    int topRight = topLeft + 1;
                    int bottomLeft = ((gz + 1) * vertex_count) + gx;
                    int bottomRight = bottomLeft + 1;
                    indices[pointer++] = topLeft;
                    indices[pointer++] = bottomLeft;
                    indices[pointer++] = topRight;
                    indices[pointer++] = topRight;
                    indices[pointer++] = bottomLeft;
                    indices[pointer++] = bottomRight;
                }
            }
            return Loader.LoadToVAO(new LoaderVertex(vertices, textureCoords, normals, indices), $"Terrain_{size}");
        }
    }
}
