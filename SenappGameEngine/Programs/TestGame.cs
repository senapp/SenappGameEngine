using System;

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
    public class TestGame : Game
    {
        public TestGame(GraphicsMode gMode) : base(WIDTH, HEIGHT, gMode, TITLE)
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
            font.LoadFont("arial");
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
            plane.transform = new Transform(size / 2, 0, size / 2);
            plane.AddComponent(new RigidEntity(plane.transform.position));

            target.AddComponent(new Entity("alduin", "alduin_img"));
            target.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            target.transform.position = new Vector3(-10, 5, 0);
            target.GetComponent<Entity>().model.hasTransparency = true;
            target.AddComponent(new RaycastTarget(5 / 0.02f, null, () => { Targeting = !Targeting; player.transform.rotation = new Vector3(0, player.transform.LookAt(target.transform.position).Y - 180, 0); },null));

            for (int i = 0; i < 200; i++)
            {
                GameObject obj = new GameObject();
                obj.AddComponent(new Entity(Geometries.Sphere, ""));
                obj.SetColour(new Vector3(Randomize.RangeFloat(0f, 1f), Randomize.RangeFloat(0f, 1f), Randomize.RangeFloat(0f, 1f)));
                obj.transform.position = new Vector3(Randomize.RangeFloat(-50f, 50f), Randomize.RangeFloat(0.5f, 50f), Randomize.RangeFloat(-50f, 50f));
                obj.transform.localScale = new Vector3(Randomize.RangeFloat(0.5f, 2f));
                obj.GetComponent<Entity>().model.reflectivity = 0.5f;
                obj.AddComponent(new RigidEntity(obj.transform.position));
            }

            player.AddComponent(new Entity("witcher", "witcher_img"));
            player.transform.localScale = new Vector3(2, 2, 2);
            player.GetComponent<Entity>().model.hasTransparency = true;


            sunLight.transform = new Transform(-200, 2000, -200, 0, 0, 0, 2f, 2f, 2f);
            sunLight.AddComponent(new Entity(Geometries.Sphere));
            sunLight.GetComponent<Entity>().model.luminosity = 1;

            mainCamera.transform = new Transform(0, 4, 10);

            var healthbar = new GameObject();
            healthbar.AddComponent(new Sprite("healthbar"));
            healthbar.transform.position = new Vector3(5,-10,0);
            healthbar.transform.localScale = new Vector3(0.8f, 100f / 256f * 0.8f, 0.5f);
            healthbar.GetComponent<Sprite>().UIConstriant = UIPosition.TopLeft;

            var minimap = new GameObject();
            minimap.AddComponent(new Sprite("minimap"));
            minimap.transform.position = new Vector3(-15, -10, 0);
            minimap.transform.localScale = new Vector3(0.8f, 128f / 256f * 0.8f, 0.5f);
            minimap.GetComponent<Sprite>().UIConstriant = UIPosition.TopRight;

            var questText = new GameObject();
            questText.AddComponent(new Text("Ladies of the Wood", font, 6));
            questText.SetColour(new Vector3(216f / 255f, 150f / 255f, 63f / 255f));
            questText.transform.position = new Vector3(45f, -55, 0);
            questText.GetComponent<Text>().UIConstriant = UIPosition.TopRight;

            var questDes1 = new GameObject();
            questDes1.AddComponent(new Text("Strange women lying", font, 4));
            questDes1.SetColour(new Vector3(249f / 255f, 212f / 255f, 144f / 255f));
            questDes1.transform.position = new Vector3(50f, -62, 0);
            questDes1.GetComponent<Text>().UIConstriant = UIPosition.TopRight;

            var questDes2 = new GameObject();
            questDes2.AddComponent(new Text("in ponds distributing", font, 4));
            questDes2.SetColour(new Vector3(249f / 255f, 212f / 255f, 144f / 255f));
            questDes2.transform.position = new Vector3(50f, -65, 0);
            questDes2.GetComponent<Text>().UIConstriant = UIPosition.TopRight;

            var questDes3 = new GameObject();
            questDes3.AddComponent(new Text("swords is no basis for", font, 4));
            questDes3.SetColour(new Vector3(249f / 255f, 212f / 255f, 144f / 255f));
            questDes3.transform.position = new Vector3(50f, -68, 0);
            questDes3.GetComponent<Text>().UIConstriant = UIPosition.TopRight;

            var questDes4 = new GameObject();
            questDes4.AddComponent(new Text("a system of government", font, 4));
            questDes4.SetColour(new Vector3(249f / 255f, 212f / 255f, 144f / 255f));
            questDes4.transform.position = new Vector3(50f, -71, 0);
            questDes4.GetComponent<Text>().UIConstriant = UIPosition.TopRight;

            float offset = 30;

            ProfilerScreen.AddComponent(new Sprite("grid"));
            ProfilerScreen.transform.localScale = new Vector3(0.6f, 0.2f, 0.5f);
            ProfilerScreen.transform.position = new Vector3(0, 7 + offset, 0);
            ProfilerScreen.GetComponent<Sprite>().UIConstriant = UIPosition.Left;

            text1.AddComponent(new Text("FPS", font, 8));
            text1.transform.position = new Vector3(0, -35 + offset, 0);
            text1.GetComponent<Text>().UIConstriant = UIPosition.Left;

            text2.AddComponent(new Text("Memory", font, 8));
            text2.transform.position = new Vector3(0, -43 + offset, 0);
            text2.GetComponent<Text>().UIConstriant = UIPosition.Left;
        }

        GameObject player = new GameObject();
        GameObject target = new GameObject();

        GameObject text1 = new GameObject();
        GameObject text2 = new GameObject();
        GameObject ProfilerScreen = new GameObject();

        private bool CameraFollowMouse;
        private bool Gaming;
        private bool Targeting;

        private void DebugScreenTexts()
        {
            text1.GetComponent<Text>().UpdateText("FPS: " + FrameRate.Get().ToString());
            text2.GetComponent<Text>().UpdateText("GameObjects: " + Game.GameObjects.Count);
            //text2.GetComponent<Text>().UpdateText(string.Format("Memory: {0}mb", MathF.Round(Process.GetCurrentProcess().PrivateMemorySize64 / 1024f / 1024f)));
        }

        private void Update(object sender, GameUpdatedEventArgs args)
        {
            if (!Focused)
                return;
            DebugScreenTexts();

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
            if (Input.GetKeyDown(Key.G) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.Start))
            {
                Gaming = !Gaming;
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

            if (!Gaming)
            {
                const float cameraSpeed = 20.0f;

                if (Input.GetKey(Key.Space))
                    mainCamera.transform.position += mainCamera.transform.Up * cameraSpeed * args.DeltaTime;
                if (Input.GetKey(Key.ShiftLeft))
                    mainCamera.transform.position -= mainCamera.transform.Up * cameraSpeed * args.DeltaTime;
                if (ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButton(Buttons.A))
                    mainCamera.transform.position += mainCamera.transform.Up * cameraSpeed * args.DeltaTime;
                if (ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButton(Buttons.B))
                    mainCamera.transform.position -= mainCamera.transform.Up * cameraSpeed * args.DeltaTime;

                if (Input.GetKey(Key.A))
                    mainCamera.transform.position -= mainCamera.transform.Right * cameraSpeed * args.DeltaTime;
                if (Input.GetKey(Key.D))
                    mainCamera.transform.position += mainCamera.transform.Right * cameraSpeed * args.DeltaTime;
                if (ControllerManager.ControllerExists(0))
                {
                    mainCamera.transform.position += mainCamera.transform.Right * cameraSpeed * args.DeltaTime * ControllerManager.GetController(0).GetAxis(Controller.Axis.HorizontalLeft);
                }

                if (Input.GetKey(Key.W))
                    mainCamera.transform.position += mainCamera.transform.Front * cameraSpeed * args.DeltaTime;
                if (Input.GetKey(Key.S))
                    mainCamera.transform.position -= mainCamera.transform.Front * cameraSpeed * args.DeltaTime;
                if (ControllerManager.ControllerExists(0))
                {
                    mainCamera.transform.position += mainCamera.transform.Front * cameraSpeed * args.DeltaTime * ControllerManager.GetController(0).GetAxis(Controller.Axis.VerticalLeft);
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

                    mainCamera.transform.Rotate(-delta.Y * sensitivity, -delta.X * sensitivity, 0);
                    mainCamera.transform.rotation = new Vector3(Math.Clamp(mainCamera.transform.rotation.X, -89f, 89f), mainCamera.transform.rotation.Y, mainCamera.transform.rotation.Z);
                }
            }
            else
            {
                const float movementSpeed = 10.0f;
                var movement = Vector3.Zero;

                if (ControllerManager.ControllerExists(0)) movement.Z = ControllerManager.GetController(0).GetAxis(Controller.Axis.VerticalLeft);
                if (Input.GetKey(Key.W)) movement.Z += 1;
                if (Input.GetKey(Key.S)) movement.Z -= 1;

                if (ControllerManager.ControllerExists(0)) movement.X = -ControllerManager.GetController(0).GetAxis(Controller.Axis.HorizontalLeft);
                if (Input.GetKey(Key.A)) movement.X += 1;
                if (Input.GetKey(Key.D)) movement.X -= 1;

                if (CameraFollowMouse)
                {
                    Vector2 delta = Input.GetMouseDelta();

                    if (ControllerManager.ControllerExists(0))
                    {
                        var newDelta = ControllerManager.GetController(0).GetAxis(2, 3);
                        if (Math.Abs(newDelta.X) > 0)
                        {
                            delta = newDelta;
                        }
                    }

                    if (Input.GetLockCursor()) Input.SetMousePositionScreenCenter(0, 0);

                     // if (delta.X < 0) mainCamera.transform.position -= mainCamera.transform.Right * 10 * args.DeltaTime;
                     // if (delta.X > 0) mainCamera.transform.position += mainCamera.transform.Right * 10 * args.DeltaTime;      
                     // mainCamera.transform.LookAt(player.transform.position);
                }
                else
                {
                    if (movement.Z != 0 || movement.X != 0)
                    {
                        var camForward = mainCamera.transform.Front;
                        camForward.Y = 0;
                        camForward.Normalize();
                        var camRight = mainCamera.transform.Right;
                        camRight.Y = 0;
                        camRight.Normalize();

                        if (!Targeting)
                        {
                            var direction = player.transform.position + (camForward * -movement.Z + camRight * movement.X).Normalized();
                            player.transform.RotateTowardsTarget(direction, args.DeltaTime * 5, 270);
                        }
                        else
                        {
                            player.transform.RotateTowardsTarget(target.transform.position, 1, 90);
                        }


                        var movementVector = new Vector3(((camForward * -movement.Z + camRight * movement.X) * movementSpeed * args.DeltaTime).X, 0, ((camForward * -movement.Z + camRight * movement.X) * movementSpeed * args.DeltaTime).Z);

                        player.transform.position -= movementVector;
                    }

                    var pos = player.transform.position;
                    mainCamera.transform.rotation = new Vector3(-15f, -180, mainCamera.transform.rotation.Z);
                    mainCamera.transform.position = new Vector3(pos.X, pos.Y + 6, pos.Z - 7);
                }
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
            mainCamera.GetComponent<Camera>().Fov -= e.DeltaPrecise;
            base.OnMouseWheel(e);
        }
    }
}
