using System.Linq;

using OpenTK;

using Senapp.Engine.Core;
using Senapp.Engine.Entities;
using Senapp.Engine.UI.Components;

namespace Senapp.Engine.Raycasts
{
    public class RaycastManager
    {
        public RaycastManager(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
        }

        public void RaycastSendingUpdate(Vector2 mousePosition)
        {
            var raycastTargets = Game.Instance.GetAllComponents<RaycastTarget>();
            raycastTargets.Sort(SortByDistanceToCamera);

            foreach (var target in raycastTargets)
            {
                var dist = Raycast.DistanceFromPoint(mousePosition, target.gameObject.transform.GetWorldPosition());
                var inRadius = dist <= target.hitRadius;

                if (inRadius && currentTargetUI == null && currentTarget != target)
                {
                    if (currentTarget != null)
                    {
                        currentTarget.onExit?.Invoke();
                        currentTarget.hovering = false;
                        currentTarget = null;
                    }

                    target.onEnter?.Invoke();
                    target.hovering = true;
                    currentTarget = target;
                }
                else if (!inRadius && currentTargetUI == null && currentTarget == target)
                {
                    target.onExit?.Invoke();
                    target.hovering = false;
                    currentTarget = null;
                }
            }
        }
        public void RaycastUISendingUpdate(Vector2 mousePosition)
        {
            var raycastTargetsUI = Game.Instance.GetAllComponents<RaycastTargetUI>();
            raycastTargetsUI = raycastTargetsUI.OrderBy(target => target.gameObject.HasComponent<Text>() 
                    ? target.gameObject.GetComponent<Text>().SortingLayer 
                    : target.gameObject.GetComponent<Sprite>().SortingLayer)
                .ToList();
            
            foreach (var target in raycastTargetsUI)
            {
                var inBox = false;

                if (target.gameObject.HasComponent<Text>())
                {
                    var text = target.gameObject.GetComponent<Text>();
                    var dimensions = text.GetUIDimensionsPixels();

                    var minX = dimensions.X;
                    var maxX = dimensions.Z;

                    var minY = dimensions.Y;
                    var maxY = dimensions.W;

                    inBox = minX <= mousePosition.X && mousePosition.X <= maxX && minY <= mousePosition.Y && mousePosition.Y <= maxY;
                }
                else
                {
                    var element = target.gameObject.GetComponent<Sprite>();
                    var dimensions = element.GetUIDimensionsPixels();

                    var minX = dimensions.X;
                    var maxX = dimensions.Z;

                    var minY = dimensions.Y;
                    var maxY = dimensions.W;

                    inBox = minX <= mousePosition.X && mousePosition.X <= maxX && minY <= mousePosition.Y && mousePosition.Y <= maxY;
                }

                if (inBox && currentTargetUI != target)
                {
                    if (currentTarget != null)
                    {
                        currentTarget.onExit?.Invoke();
                        currentTarget.hovering = false;
                        currentTarget = null;
                    }

                    if (currentTargetUI != null)
                    {
                        currentTargetUI.onExit?.Invoke();
                        currentTargetUI.hovering = false;
                        currentTargetUI = null;
                    }

                    target.onEnter?.Invoke();
                    target.hovering = true;
                    currentTargetUI = target;
                }
                else if (!inBox && target == currentTargetUI)
                {
                    currentTargetUI.onExit?.Invoke();
                    currentTargetUI.hovering = false;
                    currentTargetUI = null;
                }
            }
        }

        public void RaycastDown()
        {
            if (currentTargetUI != null)
            {
                if (currentTargetUI.hovering)
                {
                    currentTargetUI.focused = true;
                }
            }
            else if (currentTarget != null)
            {
                if (currentTarget.hovering)
                {
                    currentTarget.focused = true;
                }
            }

            foreach (var target in Game.Instance.GetAllComponents<RaycastTarget>())
            {
                if (target != currentTarget && target.focused)
                {
                    target.focused = false;
                    target.onLoseFocus?.Invoke();
                }
            }

            foreach (var target in Game.Instance.GetAllComponents<RaycastTargetUI>())
            {
                if (target != currentTargetUI && target.focused)
                {
                    target.focused = false;
                    target.onLoseFocus?.Invoke();
                }
            }
        }
        public void RaycastUp()
        {
            if (currentTargetUI != null)
            {
                if (currentTargetUI.focused)
                {
                    currentTargetUI.onClick?.Invoke();
                }
            }
            else if (currentTarget != null)
            {
                if (currentTarget.focused)
                {
                    currentTarget.onClick?.Invoke();
                }
            }
        }

        private int SortByDistanceToCamera(RaycastTarget a, RaycastTarget b)
        {
            return -Vector3.Distance(a.gameObject.transform.GetWorldPosition(), mainCamera.gameObject.transform.GetWorldPosition()).CompareTo(Vector3.Distance(b.gameObject.transform.GetWorldPosition(), mainCamera.gameObject.transform.GetWorldPosition()));
        }

        private readonly Camera mainCamera;
        private RaycastTarget currentTarget;
        private RaycastTargetUI currentTargetUI;
    }
}
