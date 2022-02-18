using System.Collections.Generic;

namespace Senapp.Engine.Renderer.Abstractions
{
    public interface IComponentRenderer<TComponent, TCommonValue>
    {
        public void Render(Dictionary<TCommonValue, List<TComponent>> renderObjects);
        public void BindCommon(TCommonValue instance);
        public void UnbindCommon();
        public void PrepareInstance(TComponent component);
    }
}
