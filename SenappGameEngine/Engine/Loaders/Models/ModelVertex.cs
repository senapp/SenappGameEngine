using OpenTK;

namespace Senapp.Engine.Loaders.Models
{
    public class ModelVertex
    {
        private const int NO_INDEX = -1;

        public Vector3 position;
        public int textureIndex = NO_INDEX;
        public int normalIndex = NO_INDEX;
        public ModelVertex duplicateVertex = null;
        public int index;
        public float length;


        public ModelVertex(int index, Vector3 position)
        {
            this.index = index;
            this.position = position;
            this.length = position.Length;
        }

        public bool IsSet()
        {
            return textureIndex != NO_INDEX && normalIndex != NO_INDEX;
        }

        public bool HasSameTextureAndNormal(int textureIndexOther, int normalIndexOther)
        {
            return textureIndexOther == textureIndex && normalIndexOther == normalIndex;
        }
    }
}
