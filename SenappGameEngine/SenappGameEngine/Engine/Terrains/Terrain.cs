using OpenTK;
using Senapp.Engine.Base;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.Terrains
{
    public class Terrain : Component
    {
        public static readonly int VERTEX_COUNT = 128;

		public RawModel model { get; set; }
		public TerrainTexture texturePack { get; set; }
		public bool isPackage = false;
		public int ID = 0;
		public Terrain() { }
		public Terrain(float size, TerrainTexture _textures)
        {
            texturePack = _textures;
			model = GenerateTerrain(size);
		}
        public static RawModel GenerateTerrain(float SIZE)
        {
            int count = VERTEX_COUNT * VERTEX_COUNT;
            float[] vertices = new float[count * 3];
            float[] normals = new float[count * 3];
            float[] textureCoords = new float[count * 2];
            int[] indices = new int[6 * (VERTEX_COUNT - 1) * (VERTEX_COUNT * 1)];
			int vertexPointer = 0;
			for (int i = 0; i < VERTEX_COUNT; i++)
			{
				for (int j = 0; j < VERTEX_COUNT; j++)
				{

					vertices[vertexPointer * 3] = -SIZE + (float)j / ((float)VERTEX_COUNT - 1) * SIZE;
					vertices[vertexPointer * 3 + 1] = 0;
					vertices[vertexPointer * 3 + 2] = -SIZE + (float)i / ((float)VERTEX_COUNT - 1) * SIZE;
					normals[vertexPointer * 3] = 0;
					normals[vertexPointer * 3 + 1] = 1;
					normals[vertexPointer * 3 + 2] = 0;
					textureCoords[vertexPointer * 2] = (float)j / ((float)VERTEX_COUNT - 1);
					textureCoords[vertexPointer * 2 + 1] = (float)i / ((float)VERTEX_COUNT - 1);
					vertexPointer++;
				}
			}
			int pointer = 0;
			for (int gz = 0; gz < VERTEX_COUNT - 1; gz++)
			{
				for (int gx = 0; gx < VERTEX_COUNT - 1; gx++)
				{
					int topLeft = (gz * VERTEX_COUNT) + gx;
					int topRight = topLeft + 1;
					int bottomLeft = ((gz + 1) * VERTEX_COUNT) + gx;
					int bottomRight = bottomLeft + 1;
					indices[pointer++] = topLeft;
					indices[pointer++] = bottomLeft;
					indices[pointer++] = topRight;
					indices[pointer++] = topRight;
					indices[pointer++] = bottomLeft;
					indices[pointer++] = bottomRight;
				}
			}
			return Loader.LoadToVAO(vertices, textureCoords, normals, indices);
		}
    }
}
