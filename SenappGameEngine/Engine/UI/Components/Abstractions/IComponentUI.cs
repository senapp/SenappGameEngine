using OpenTK;

namespace Senapp.Engine.UI.Components.Abstractions
{
    public interface IComponentUI
    {
        public Vector4 GetUIDimensionsPixels();
        public Vector3 GetUIPosition();
    }
}
