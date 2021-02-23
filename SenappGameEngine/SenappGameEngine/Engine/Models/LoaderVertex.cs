using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.Models
{
    public class LoaderVertex
    {
        public float[] positions;
        public float[] textureCoords;
        public float[] normals;
        public int[] indices;
        public float furthestPoint;


        public LoaderVertex(float[] pos, float[] texCor, float[] norms, int[] ind, float furthestpoint = 0)
        {
            positions = pos;
            textureCoords = texCor;
            normals = norms;
            indices = ind;
            furthestPoint = furthestpoint;
        }
    }
}
