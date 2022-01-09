using System.Collections.Generic;
using System.Linq;

using OpenTK;
using OpenTK.Input;

using Senapp.Engine.Base;
using Senapp.Engine.Entities;
using Senapp.Engine.UI;

namespace Senapp.Engine.Physics
{
    public class RaycastManager
    {
        private int SortByDistanceToCamera(GameObject a, GameObject b)
        {
            return Vector3.Distance(a.transform.position, Game.mainCamera.transform.position).CompareTo(Vector3.Distance(b.transform.position, Game.mainCamera.transform.position));
        }
        private int SortByZ(GameObject a, GameObject b)
        {
            return a.transform.position.Z.CompareTo(b.transform.position.Z);
        }

        private RaycastTarget currentTarget;
        private RaycastTargetUI currentTargetUI;

        public void RaycastSendingUpdate(MouseMoveEventArgs e)
        {
            var cam = Game.mainCamera.GetComponent<Camera>();
            var sortedObjects = new List<GameObject>(Game.GameObjects);
            sortedObjects.Sort(SortByDistanceToCamera);
            foreach (var gameObject in sortedObjects)
            {
                if (gameObject.HasComponent<RaycastTarget>() && gameObject.enabled)
                {
                    var target = gameObject.GetComponent<RaycastTarget>();
                    float dist = Raycast.DistanceFromPoint(new Vector2(e.X, e.Y), new Vector3(0, 0, 0), gameObject.transform.TransformationMatrix() * cam.GetViewMatrix(), cam.GetProjectionMatrix());

                    if (dist <= target.hitRadius && !target.hovering && currentTarget == null && currentTargetUI == null)
                    {
                        if (target.onEnter != null) target.onEnter();
                        target.hovering = true;
                        currentTarget = target;
                        return;
                    }
                    else if (dist <= target.hitRadius && !target.hovering && currentTarget != null && currentTargetUI == null)
                    {
                        if (Vector3.Distance(currentTarget.gameObject.transform.position, Game.mainCamera.transform.position) > Vector3.Distance(target.gameObject.transform.position, Game.mainCamera.transform.position))
                        {
                            if (currentTarget.onExit != null) currentTarget.onExit();
                            currentTarget.hovering = false;
                            if (target.onEnter != null) target.onEnter();
                            target.hovering = true;
                            currentTarget = target;
                            return;
                        }
                    }
                    else if (dist > target.hitRadius && target.hovering && currentTarget != null && currentTargetUI == null)
                    {
                        if (target.onExit != null) target.onExit();
                        target.hovering = false;
                        currentTarget = null;
                        return;
                    }
                }
            }
        }
        public void RaycastUISendingUpdate(MouseMoveEventArgs e)
        {
            var sortedObjects = new List<GameObject>(Game.GameObjects);
            sortedObjects.Sort(SortByZ);
            foreach (var gameObject in sortedObjects)
            {
                if (gameObject.HasComponent<RaycastTargetUI>() && gameObject.enabled)
                {
                    var target = gameObject.GetComponent<RaycastTargetUI>();
                    var inBox = false;

                    if (target.gameObject.HasComponent<Text>())
                    {
                        var text = target.gameObject.GetComponent<Text>();
                        var dimensions = text.GetUIDimensionsPixels(true, text);

                        var minX = dimensions.X;
                        var maxX = dimensions.Z;

                        var minY = dimensions.Y;
                        var maxY = dimensions.W;

                        inBox = minX <= e.X && e.X <= maxX && minY <= e.Y && e.Y <= maxY;
                    }
                    else if (target.gameObject.HasComponent<Sprite>())
                    {
                        var element = target.gameObject.GetComponent<Sprite>();
                        var dimensions = element.GetUIDimensionsPixels(false);

                        var minX = dimensions.X;
                        var maxX = dimensions.Z;

                        var minY = dimensions.Y;
                        var maxY = dimensions.W;

                        inBox = minX <= e.X && e.X <= maxX && minY <= e.Y && e.Y <= maxY;
                    }

                    if (inBox && !target.hovering && currentTargetUI == null)
                    {
                        if (currentTarget != null)
                        {
                            currentTarget.onExit?.Invoke();
                            currentTarget.hovering = false;
                            currentTarget = null;
                        }

                        target.onEnter?.Invoke();
                        target.hovering = true;
                        currentTargetUI = target;
                        return;
                    }
                    else if (!inBox && !target.hovering && currentTargetUI != null)
                    {
                        if (currentTargetUI.gameObject.transform.position.Z > target.gameObject.transform.position.Z)
                        {
                            currentTargetUI.onExit?.Invoke();
                            currentTargetUI.hovering = false;

                            target.onEnter?.Invoke();
                            target.hovering = true;
                            currentTargetUI = target;
                            return;
                        }
                    }
                    else if (!inBox && target.hovering && currentTargetUI != null)
                    {
                        target.onExit?.Invoke();
                        target.hovering = false;
                        currentTargetUI = null;
                        return;
                    }
                }
            }
        }
        public void RaycastClickCheck()
        {
            if (currentTargetUI != null)
            {
                if (currentTargetUI.hovering)
                {
                    currentTargetUI.focused = true;
                    currentTargetUI.onClick?.Invoke();
                }
            }
            else if (currentTarget != null)
            {
                if (currentTarget.hovering)
                {
                    currentTarget.focused = true;
                    currentTarget.onClick?.Invoke();
                }
            }

            foreach (var gameObject in Game.GameObjects.Where(obj => obj.enabled))
            {
                if (gameObject.HasComponent<RaycastTargetUI>())
                {
                    var target = gameObject.GetComponent<RaycastTargetUI>();
                    if (target != currentTargetUI && target.focused)
                    {
                        target.focused = false;
                        target.onLoseFocus?.Invoke();
                    }
                }
                else if (gameObject.HasComponent<RaycastTarget>())
                {
                    var target = gameObject.GetComponent<RaycastTarget>();
                    if (target != currentTarget && target.focused)
                    {
                        target.focused = false;
                        target.onLoseFocus?.Invoke();
                    }
                }
            }
        }
    }
}
