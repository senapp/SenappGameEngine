using OpenTK;
using Senapp.Engine.Base;
using Senapp.Engine.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.Physics
{
    public class Rigidbody : Component
    {
        public static readonly float gravity = -9.81f;
        public float multiplier = 1;
        public bool falling = true;


        public Rigidbody() { }
        public Rigidbody(float multiplier)
        {
            this.multiplier = multiplier;
        }
        public override void Update(GameUpdatedEventArgs args)
        {
            if (falling) gameObject.transform.Translate(0, gravity * args.DeltaTime * multiplier,0);
        }
        public override bool ComponentConditions(GameObject gameObject)
        {
            return gameObject.HasComponent<BoxCollisionMesh>();
        }
    }
}
