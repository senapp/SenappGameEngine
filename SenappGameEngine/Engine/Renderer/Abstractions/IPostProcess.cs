namespace Senapp.Engine.Renderer.Abstractions
{
    interface IPostProcess
    {
        public int Render(int texture);
        public void OnResize(int width, int height);
        public void Dispose();
    }
}
