using OpenTK;
using Senapp.Engine.Core.Components;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Entities;
using Senapp.Engine.Events;
using Senapp.Engine.PlayerInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senapp.Programs.Moba
{
    public class Tower : Component
    {
        public bool LeftTeam;

        public Tower() { }

        public Tower(Vector3 position, Vector3 rotation, bool leftTeam)
        {
            LeftTeam = leftTeam;
            startPos = position;
            startRotation = rotation;

            model = new Entity("tower");
        }

        public override void Awake()
        {
            model = new GameObject()
                   .WithParent(gameObject)
                   .WithPosition(startPos)
                   .WithRotation(startRotation)
                   .WithScale(new Vector3(0.2f))
                   .WithName(gameObject.name + " Model")
                   .AddComponent(model);
        }

        public override void Update(GameUpdatedEventArgs args)
        {
        }

        private Vector3 startPos;
        private Vector3 startRotation;
        private Entity model;
    }
}
