namespace Senapp.Engine.Loaders
{
    public class LoaderVertex
    {
        public float[] positions;
        public float[] textureCoords;
        public float[] normals;
        public int[] indices;

        public LoaderVertex(float[] pos, float[] texCor, float[] norms, int[] ind)
        {
            positions = pos;
            textureCoords = texCor;
            normals = norms;
            indices = ind;
        }
    }
}
