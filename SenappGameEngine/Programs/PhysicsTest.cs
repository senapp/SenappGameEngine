using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

using Senapp.Engine.Base;
using Senapp.Engine.Entities;
using Senapp.Engine.Events;
using Senapp.Engine.Physics;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.Renderer;
using Senapp.Engine.Terrains;
using Senapp.Engine.UI;
using Senapp.Engine.Utilities;

namespace Senapp.Programs
{
    public class PhysicsTest : Game
    {
        public PhysicsTest(GraphicsMode gMode) : base(WIDTH, HEIGHT, gMode, TITLE)
        {
            GameInitializedEvent += Initialize;
            GameResizeEvent += Resized;
            GameUpdatedEvent += Update;
            GameRenderedEvent += Render;
            GameClosedEvent += Close;
            Run();
        }

        public static readonly bool START_FULLSCREEN = false;
        public static readonly int WIDTH = 800;
        public static readonly int HEIGHT = 600;
        public static readonly string TITLE = "Test";

        public GameFont font = new GameFont();
        private void Initialize(object sender)
        {
            font.LoadFont("opensans");
            Icon = Resources.GetIcon("new_icon");

            WindowInitialization();
            GameInitilaztion();
        }
        private void WindowInitialization()
        {
            VSync = VSyncMode.On;
            if (START_FULLSCREEN)
                WindowState = WindowState.Fullscreen;
        }
        private void GameInitilaztion()
        {
            GameObject plane = new GameObject();
            int size = 100;
            plane.AddComponent(new Entity(Terrain.GenerateTerrain(size), ""));
            plane.isStatic = true;
            plane.transform = new Transform(size / 2, 2, size / 2);
            plane.AddComponent(new RigidEntity(plane.transform.position));
            MainScene.AddGameObject(plane);

            for (int i = 0; i < 100; i++)
            {
                GameObject obj = new GameObject();
                obj.AddComponent(new Entity(Geometries.Cube, ""));
                obj.transform.position = new Vector3(Randomize.RangeFloat(1, 10), Randomize.RangeFloat(1, 10), Randomize.RangeFloat(1, 10));
                obj.GetComponent<Entity>().model.shineDamper = 0.1f;
                obj.GetComponent<Entity>().model.reflectivity = 0.1f;
                obj.GetComponent<Entity>().model.luminosity = 0.8f;
                obj.AddComponent(new RigidEntity(obj.transform.position));
                obj.AddComponent(new RaycastTarget(1, onClick: () =>
                {
                    obj.SetColour(Color.Red);
                    lockToMouseObject = obj;
                }, onLoseFocus: () =>
                {
                    obj.SetColour(Color.White);
                    if (lockToMouseObject == obj)
                    {
                        lockToMouseObject = null;
                    }
                }));
                MainScene.AddGameObject(obj);
            }

            SunLight.gameObject.transform = new Transform(0, 25, 0, 0, 0, 0, 2f, 2f, 2f);
            SunLight.gameObject.AddComponent(new Entity(Geometries.Sphere));
            SunLight.gameObject.GetComponent<Entity>().model.luminosity = 1;

            MainCamera.gameObject.transform = new Transform(0, 4, 10);

            float offset = 30;

            ProfilerScreen.AddComponent(new Sprite("grid"));
            ProfilerScreen.transform.localScale = new Vector3(0.6f, 0.2f, 0.5f);
            ProfilerScreen.transform.position = new Vector3(0, 7 + offset, 0);
            ProfilerScreen.GetComponent<Sprite>().UIConstriant = UIPosition.Left;
            MainScene.AddGameObject(ProfilerScreen);

            text1.AddComponent(new Text("FPS", font, 8));
            text1.transform.position = new Vector3(0, -35 + offset, 0);
            text1.GetComponent<Text>().UIConstriant = UIPosition.Left;
            MainScene.AddGameObject(text1);

            text2.AddComponent(new Text("Memory", font, 8));
            text2.transform.position = new Vector3(0, -43 + offset, 0);
            text2.GetComponent<Text>().UIConstriant = UIPosition.Left;
            MainScene.AddGameObject(text2);
        }


        GameObject text1 = new GameObject();
        GameObject text2 = new GameObject();
        GameObject ProfilerScreen = new GameObject();


        GameObject lockToMouseObject;

        private bool CameraFollowMouse;

        private void DebugScreenTexts()
        {
            text1.GetComponent<Text>().UpdateText("FPS: " + FrameRate.Get().ToString());
            text2.GetComponent<Text>().UpdateText("GameObjects: " + GetAllGameObjects().Count);
        }

        private void Update(object sender, GameUpdatedEventArgs args)
        {
            if (!Focused)
                return;
            DebugScreenTexts();
            if (lockToMouseObject != null)
            {
                if (Input.GetMouseButtonDown(MouseButton.Right))
                {
                    var component = lockToMouseObject.GetComponent<RaycastTarget>();
                    component.focused = false;
                    component.onLoseFocus();
                    lockToMouseObject = null;
                    return;
                }

                if (!Input.GetKey(Key.R))
                {
                    var mult = 5.25f;
                    if (Input.GetKey(Key.Up))
                        lockToMouseObject.transform.position += lockToMouseObject.transform.Up * args.DeltaTime * mult;
                    if (Input.GetKey(Key.Down))
                        lockToMouseObject.transform.position -= lockToMouseObject.transform.Up * args.DeltaTime * mult;
                    if (Input.GetKey(Key.Left))
                        lockToMouseObject.transform.position -= lockToMouseObject.transform.Right * args.DeltaTime * mult;
                    if (Input.GetKey(Key.Right))
                        lockToMouseObject.transform.position += lockToMouseObject.transform.Right * args.DeltaTime * mult;
                    if (Input.GetKey(Key.ControlRight))
                        lockToMouseObject.transform.position -= lockToMouseObject.transform.Front * args.DeltaTime * mult;
                    if (Input.GetKey(Key.ShiftRight))
                        lockToMouseObject.transform.position += lockToMouseObject.transform.Front * args.DeltaTime * mult;
                }
                else
                {
                    var mult = 20;
                    if (Input.GetKey(Key.Up))
                        lockToMouseObject.transform.Rotate(lockToMouseObject.transform.Up * args.DeltaTime * mult);
                    if (Input.GetKey(Key.Down))
                        lockToMouseObject.transform.Rotate(-lockToMouseObject.transform.Up * args.DeltaTime * mult);
                    if (Input.GetKey(Key.Left))
                        lockToMouseObject.transform.Rotate(-lockToMouseObject.transform.Right * args.DeltaTime * mult);
                    if (Input.GetKey(Key.Right))
                        lockToMouseObject.transform.Rotate(lockToMouseObject.transform.Right * args.DeltaTime * mult);
                    if (Input.GetKey(Key.ControlRight))
                        lockToMouseObject.transform.Rotate(-lockToMouseObject.transform.Front * args.DeltaTime * mult);
                    if (Input.GetKey(Key.ShiftRight))
                        lockToMouseObject.transform.Rotate(lockToMouseObject.transform.Front * args.DeltaTime * mult);
                }
            }


            if (Input.GetKeyDown(Key.Escape) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.Back))
            {
                Exit();
            }
            if (Input.GetKeyDown(Key.F) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadDown))
            {
                FrameRate.Enable(!FrameRate.IsEnabled());
            }
            if (Input.GetKeyDown(Key.Z) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadUp))
            {
                WireFrame.Enable(!WireFrame.IsEnabled());
            }
            if (Input.GetKeyDown(Key.Q) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadLeft))
            {
                ProfilerScreen.enabled = !ProfilerScreen.enabled;
                text1.enabled = !text1.enabled;
                text2.enabled = !text2.enabled;
            }
            if (Input.GetKeyDown(Key.V) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadRight))
            {
                if (VSync == VSyncMode.Off) VSync = VSyncMode.On;
                else VSync = VSyncMode.Off;
            }
            if (Input.GetKeyDown(Key.L) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.LeftShoulder))
            {
                Input.LockCursor(!Input.GetLockCursor());
                Input.ShowCursor(!Input.GetCursorVisibility());
                CameraFollowMouse = !CameraFollowMouse;
                if (Input.GetLockCursor())
                    Input.SetMousePositionScreenCenter(0, 0);
            }

            const float cameraSpeed = 5;

            if (Input.GetKey(Key.Space))
                MainCamera.gameObject.transform.position += MainCamera.gameObject.transform.Up * cameraSpeed * args.DeltaTime;
            if (Input.GetKey(Key.ShiftLeft))
                MainCamera.gameObject.transform.position -= MainCamera.gameObject.transform.Up * cameraSpeed * args.DeltaTime;
            if (ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButton(Buttons.A))
                MainCamera.gameObject.transform.position += MainCamera.gameObject.transform.Up * cameraSpeed * args.DeltaTime;
            if (ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButton(Buttons.B))
                MainCamera.gameObject.transform.position -= MainCamera.gameObject.transform.Up * cameraSpeed * args.DeltaTime;

            if (Input.GetKey(Key.A))
                MainCamera.gameObject.transform.position -= MainCamera.gameObject.transform.Right * cameraSpeed * args.DeltaTime;
            if (Input.GetKey(Key.D))
                MainCamera.gameObject.transform.position += MainCamera.gameObject.transform.Right * cameraSpeed * args.DeltaTime;
            if (ControllerManager.ControllerExists(0))
            {
                MainCamera.gameObject.transform.position += MainCamera.gameObject.transform.Right * cameraSpeed * args.DeltaTime * ControllerManager.GetController(0).GetAxis(Controller.Axis.HorizontalLeft);
            }

            if (Input.GetKey(Key.W))
                MainCamera.gameObject.transform.position += MainCamera.gameObject.transform.Front * cameraSpeed * args.DeltaTime;
            if (Input.GetKey(Key.S))
                MainCamera.gameObject.transform.position -= MainCamera.gameObject.transform.Front * cameraSpeed * args.DeltaTime;
            if (ControllerManager.ControllerExists(0))
            {
                MainCamera.gameObject.transform.position += MainCamera.gameObject.transform.Front * cameraSpeed * args.DeltaTime * ControllerManager.GetController(0).GetAxis(Controller.Axis.VerticalLeft);
            }

            if (CameraFollowMouse)
            {
                Vector2 delta = Input.GetMouseDelta();
                var sensitivity = Camera.Sensitivity;
                if (ControllerManager.ControllerExists(0))
                {
                    var newDelta = ControllerManager.GetController(0).GetAxis(3, 4);
                    if (Math.Abs(newDelta.X) + Math.Abs(newDelta.Y) > 0)
                    {
                        delta = newDelta;
                        sensitivity = 100 * args.DeltaTime;
                    }
                }         

                if (Input.GetLockCursor()) Input.SetMousePositionScreenCenter(0, 0);

                MainCamera.gameObject.transform.Rotate(-delta.Y * sensitivity, -delta.X * sensitivity, 0);
                MainCamera.gameObject.transform.rotation = new Vector3(Math.Clamp(MainCamera.gameObject.transform.rotation.X, -89f, 89f), MainCamera.gameObject.transform.rotation.Y, MainCamera.gameObject.transform.rotation.Z);
            }
        }
        private void Render(object sender, GameRenderedEventArgs e) { }
        private void Resized(object sender) { }
        private void Close(object sender) { }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            MainCamera.Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
        }
    }
}
