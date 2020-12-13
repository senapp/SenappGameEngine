using System.Collections.Generic;

namespace Senapp.Engine.Models
{
    public class RawModel
    {
        public int vaoID { get; private set; }
        public int vertexCount { get; private set; }
        public List<int> vboIDs { get; private set; }

        public RawModel(int _vaoID, int _vertexCount, List<int> _vboIDs)
        {
            vaoID = _vaoID;
            vertexCount = _vertexCount;
            vboIDs = _vboIDs;
        }
        public void Dispose()
        {
            Loader.DisposeModel(this);
        }
    }
}
