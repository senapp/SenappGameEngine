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
    }
}
