using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BulletSharp;
using Microsoft.VisualBasic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Senapp.Engine.Core;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Entities;
using Senapp.Engine.Events;
using Senapp.Engine.Physics;
using Senapp.Engine.Raycasts;
using Senapp.Engine.Renderer.Helper;
using Senapp.Engine.Utilities;
using Vector3 = OpenTK.Vector3;

namespace Senapp.Programs
{
    public class BulletSharpDemo : Game
    {
        private Physics _physics;
        private float _frameTime;
        private int _fps;

        public BulletSharpDemo(GraphicsMode GRAPHICS_MODE) : base(WIDTH, HEIGHT, GRAPHICS_MODE, TITLE)
        {
            GameInitializedEvent += Initialize;
            GameUpdatedEvent += Update;
            GameRenderedEvent += Render;
            GameClosedEvent += Close;
            Run();
        }

        public static readonly bool START_FULLSCREEN = false;
        public static readonly int WIDTH = 800;
        public static readonly int HEIGHT = 600;
        public static readonly string TITLE = "Test";

        private List<GameObject> collisionObjects = new();

        private void Initialize(object sender)
        {
            VSync = VSyncMode.Off;
            _physics = new Physics();

            foreach (RigidBody body in _physics.World.CollisionObjectArray)
            {
                if ("Ground".Equals(body.UserObject))
                {
                    collisionObjects.Add(new GameObject().WithComponent(new Entity(Geometries.Cube)).WithColour(Color.Green).WithScale(new Vector3(100)));
                    body.UserIndex = collisionObjects.Last().InstanceId;
                    continue;
                }

                var obj = new GameObject()
                       .WithComponent(new Entity(Geometries.Cube))
                       .WithColour(Color.HotPink)
                       .WithScale(new Vector3(2));

                obj.WithComponent(new RaycastTarget(1, onEnter: () =>
                {
                    obj.colour = Color.Red;
                }, onExit: () =>
                {
                    obj.colour = Color.White;
                }, onClick: () =>
                {
                    body.ApplyForce(new BulletSharp.Math.Vector3(Randomize.Range(-2, 2)), BulletSharp.Math.Vector3.Zero);
                } ));
                collisionObjects.Add(obj);
                body.UserIndex = collisionObjects.Last().InstanceId;
            }

            foreach(var item in collisionObjects)
            {
                MainScene.AddGameObject(item);
            }
        }

        private void Close(object sender)
        {
            _physics.ExitPhysics();
        }

        private void Update(object sender, GameUpdatedEventArgs args)
        {
            _physics.Update((float)args.DeltaTime);

            var keyboard = Keyboard.GetState();
            if (keyboard[Key.Escape] || keyboard[Key.Q])
                Exit();

            foreach (RigidBody body in _physics.World.CollisionObjectArray)
            {
                collisionObjects.Find(item => item.InstanceId == body.UserIndex).transform.SetMatrix(Convert(body.WorldTransform));
            }
        }

        private void Render(object sender, GameRenderedEventArgs args)
        {
            _frameTime += (float)args.DeltaTime;
            _fps++;
            if (_frameTime >= 1)
            {
                _frameTime = 0;
                Title = $"BulletSharp OpenTK Demo, FPS = {_fps}";
                _fps = 0;
            }
        }

        private static Matrix4 Convert(BulletSharp.Math.Matrix m)
        {
            return new Matrix4(
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44);
        }
    }
}