using System;

using OpenTK;
using OpenTK.Input;

using Senapp.Engine.Core;
using Senapp.Engine.Core.Components;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Entities;
using Senapp.Engine.Events;
using Senapp.Engine.PlayerInput;

namespace Senapp.Engine.Controllers
{
    public class CameraController : Component
    {
        public bool CameraFollowMouse;
        public override void Update(GameUpdatedEventArgs args)
        {
            if (Input.GetKeyDown(Key.L) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.LeftShoulder))
            {
                Input.LockCursor(!Input.CursorLocked);
                Input.ShowCursor(!Input.GetCursorVisibility());
                CameraFollowMouse = !CameraFollowMouse;
                if (Input.CursorLocked)
                    Input.SetMousePositionWindowCenter(0, 0);
            }

            const float cameraSpeed = 5;

            if (Input.GetKey(Key.Space))
                Game.Instance.MainCamera.gameObject.transform.Translate(Game.Instance.MainCamera.gameObject.transform.Up * cameraSpeed * args.DeltaTime);
            if (Input.GetKey(Key.ShiftLeft))
                Game.Instance.MainCamera.gameObject.transform.Translate(-Game.Instance.MainCamera.gameObject.transform.Up * cameraSpeed * args.DeltaTime);
            if (ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButton(Buttons.A))
                Game.Instance.MainCamera.gameObject.transform.Translate(Game.Instance.MainCamera.gameObject.transform.Up * cameraSpeed * args.DeltaTime);
            if (ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButton(Buttons.B))
                Game.Instance.MainCamera.gameObject.transform.Translate(-Game.Instance.MainCamera.gameObject.transform.Up * cameraSpeed * args.DeltaTime);

            if (Input.GetKey(Key.A))
                Game.Instance.MainCamera.gameObject.transform.Translate(-Game.Instance.MainCamera.gameObject.transform.Right * cameraSpeed * args.DeltaTime);
            if (Input.GetKey(Key.D))
                Game.Instance.MainCamera.gameObject.transform.Translate(Game.Instance.MainCamera.gameObject.transform.Right * cameraSpeed * args.DeltaTime);
            if (ControllerManager.ControllerExists(0))
            {
                Game.Instance.MainCamera.gameObject.transform.Translate(Game.Instance.MainCamera.gameObject.transform.Right * cameraSpeed * args.DeltaTime * ControllerManager.GetController(0).GetAxis(Axis.HorizontalLeft));
            }

            if (Input.GetKey(Key.W))
                Game.Instance.MainCamera.gameObject.transform.Translate(Game.Instance.MainCamera.gameObject.transform.Front * cameraSpeed * args.DeltaTime);
            if (Input.GetKey(Key.S))
                Game.Instance.MainCamera.gameObject.transform.Translate(-Game.Instance.MainCamera.gameObject.transform.Front * cameraSpeed * args.DeltaTime);
            if (ControllerManager.ControllerExists(0))
            {
                Game.Instance.MainCamera.gameObject.transform.Translate(Game.Instance.MainCamera.gameObject.transform.Front * cameraSpeed * args.DeltaTime * ControllerManager.GetController(0).GetAxis(Axis.VerticalLeft));
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

                if (Input.CursorLocked) Input.SetMousePositionWindowCenter(0, 0);

                Game.Instance.MainCamera.gameObject.transform.Rotate(-delta.Y * sensitivity, -delta.X * sensitivity, 0);
                var rotation = Game.Instance.MainCamera.gameObject.transform.GetWorldRotation();
                Game.Instance.MainCamera.gameObject.transform.SetRotation(new Vector3(Math.Clamp(rotation.X, -89f, 89f), rotation.Y, rotation.Z));
            }
        }
        public override bool ComponentConditions(GameObject gameObject)
        {
            return base.ComponentConditions(gameObject) && gameObject.HasComponent<Camera>();
        }
    }
}
