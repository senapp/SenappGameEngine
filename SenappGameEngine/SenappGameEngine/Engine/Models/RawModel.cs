using System.Collections.Generic;
using static Senapp.Engine.Models.Loader;

namespace Senapp.Engine.Models
{
    public class RawModel
    {
        public int vaoID { get; private set; }
        public int vertexCount { get; private set; }
        public List<int> vboIDs { get; private set; }
        public LoaderVertex modelData { get; private set; }

        public RawModel(int _vaoID, int _vertexCount, List<int> _vboIDs, LoaderVertex data)
        {
            vaoID = _vaoID;
            vertexCount = _vertexCount;
            vboIDs = _vboIDs;
            modelData = data;
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
