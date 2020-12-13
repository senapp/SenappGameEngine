using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using Senapp.Engine.Base;

namespace Senapp.Engine.Entities
{
    public class Light : Component
    {
        public Vector3 colour { get; set; }

        public Light()
        {

        }
        public Light(Vector3 Colour) 
        {
            colour = Colour;
        }
    }
}
