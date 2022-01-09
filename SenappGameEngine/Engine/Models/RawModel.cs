using System.Collections.Generic;

namespace Senapp.Engine.Models
{
    public class RawModel
    {
        public int vaoID { get; private set; }
        public int vertexCount { get; private set; }
        public List<int> vboIDs { get; private set; }
        public LoaderVertex modelData { get; private set; }
        public string rawModelName { get; private set; }

        public RawModel(int _vaoID, int _vertexCount, List<int> _vboIDs, LoaderVertex data, string modelName)
        {
            vaoID = _vaoID;
            vertexCount = _vertexCount;
            vboIDs = _vboIDs;
            modelData = data;
            rawModelName = modelName;
        }
        public RawModel(int _vaoID, int _vertexCount)
        {
            vaoID = _vaoID;
            vertexCount = _vertexCount;
        }
        public void Dispose()
        {
            Loader.DisposeModel(this);
        }
    }
}
