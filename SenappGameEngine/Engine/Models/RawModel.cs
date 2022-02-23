using System.Collections.Generic;

using Senapp.Engine.Loaders;

namespace Senapp.Engine.Models
{
    public class RawModel
    {
        public int VaoId { get; private set; }
        public int VertexCount { get; private set; }
        public List<int> VboIds { get; private set; }
        public LoaderVertex ModelData { get; private set; }
        public string Name { get; private set; }

        public RawModel(int vaoId, int vertexCount, List<int> vboIds, LoaderVertex data, string modelName)
        {
            VaoId = vaoId;
            VertexCount = vertexCount;
            VboIds = vboIds;
            ModelData = data;
            Name = modelName;
        }
        public RawModel(int vaoId, int vertexCount)
        {
            VaoId = vaoId;
            VertexCount = vertexCount;
        }

        public void Dispose()
        {
            Loader.DisposeModel(this);
        }

		public const int VERTEX_COUNT = 128;
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
			return Loader.LoadToVAO(new LoaderVertex(vertices, textureCoords, normals, indices), $"Terrain_{SIZE}");
		}
	}
}
