using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;

using Senapp.Engine.Controllers;
using Senapp.Engine.Core;
using Senapp.Engine.Core.Components;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Events;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.Raycasts;
using static Senapp.Engine.Utilities.VectorExtensions;

namespace Senapp.Programs.Moba
{
    public class MobaPlayerController : Component
    {
        public static MobaPlayerController Instance;

        public Vector3 CurrentRaycastPosition { get; private set; }

        public MobaPlayerController() { }
        public MobaPlayerController(GameObject targetMarker)
        {
            Instance = this;
            this.targetMarker = targetMarker;
        }

        public override void Update(GameUpdatedEventArgs args)
        {
            if (Input.GetKeyDown(Key.G))
            {
                Gaming = !Gaming;
                Game.Instance.MainCamera.gameObject.GetComponent<CameraController>().enabled = !Gaming;
            }
            if (Gaming)
            {
                CurrentRaycastPosition = Raycast.ClosestPoint(Input.GetMousePositionWindow(), 0);
                const float movementSpeed = 5.0f;

                if (Input.GetMouseButtonDown(MouseButton.Right))
                {
                    var mouseTarget = CurrentRaycastPosition.WithY(0.01f);
                    if (MobaWorld.Instance.IsPositionValid(mouseTarget))
                    {
                        comingPositions = MobaWorld.Instance.CalculateMovement(gameObject.transform.LocalPosition, CurrentRaycastPosition.WithY(0.01f));
                        comingPositions.RemoveAt(0);
                        if (comingPositions.Count == 0)
                        {
                            return;
                        }
                        targetPosition = comingPositions[0];
                        var dir = targetPosition - gameObject.transform.LocalPosition;
                        movement = -dir.Normalized().WithY(0);
                        targetMarker.transform.SetPosition(mouseTarget);
                        targetMarker.enabled = true;
                        comingPositions.RemoveAt(0);
                    }
                }

                if (IsOnPosition(gameObject.transform.LocalPosition, targetPosition))
                {
                    if (comingPositions.Count == 0)
                    {
                        movement = Vector3.Zero;
                        targetPosition = Vector3.Zero;
                        targetMarker.enabled = false;
                    }
                    else
                    {
                        targetPosition = comingPositions[0];
                        comingPositions.RemoveAt(0);
                    }
                }
                else if (targetPosition != Vector3.Zero)
                {
                    var dir = targetPosition - gameObject.transform.LocalPosition;
                    movement = -dir.Normalized().WithY(0);
                }

                if (movement.Z != 0 || movement.X != 0)
                {
                    var camForward = Game.Instance.MainCamera.gameObject.transform.Front;
                    camForward.Y = 0;
                    camForward.Normalize();
                    var camRight = Game.Instance.MainCamera.gameObject.transform.Right;
                    camRight.Y = 0;
                    camRight.Normalize();

                    var direction = gameObject.transform.GetWorldPosition() + (camForward * -movement.Z + camRight * movement.X).Normalized();
                    gameObject.transform.RotateTowardsTarget(direction, args.DeltaTime * 5, 90);

                    var movementVector = new Vector3(((camForward * -movement.Z + camRight * movement.X).Normalized() * movementSpeed * args.DeltaTime).X, 0, ((camForward * -movement.Z + camRight * movement.X).Normalized() * movementSpeed * args.DeltaTime).Z);
                    gameObject.transform.Translate(-movementVector);
                }

                var pos = gameObject.transform.GetWorldPosition();
                Game.Instance.MainCamera.gameObject.transform.SetRotation(new Vector3(-40f - cameraOffset * 4f, 0, 180));
                Game.Instance.MainCamera.gameObject.transform.SetPosition(new Vector3(pos.X, pos.Y + 3.5f + (-MathF.Pow(cameraOffset - 4, 2)) + cameraOffset, pos.Z + cameraOffset + 2 + (-MathF.Pow(cameraOffset - 4, 2))));
            }

        }

        public void OnScroll(float delta)
        {
            cameraOffset += delta / 10f;
            cameraOffset = Math.Clamp(cameraOffset, 2.3f, 3.3f);
        }

        private bool Gaming = true;
        private Vector3 targetPosition = Vector3.Zero;
        private Vector3 movement = Vector3.Zero;
        private readonly GameObject targetMarker;
        private float cameraOffset = 3.3f;
        private List<Vector3> comingPositions = new();
    }
}
