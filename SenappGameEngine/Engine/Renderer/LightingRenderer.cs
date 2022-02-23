using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Core;
using Senapp.Engine.Renderer.FrameBuffers;
using Senapp.Engine.Shaders;

namespace Senapp.Engine.Renderer
{
    public class LightingRenderer : FrameBufferRenderer
	{
		public LightingRenderer()
		{
			this.shader = new LightingShader();
		}

		public void Bind(int colourTexture, int normalTexture, int positionTexture, int modelTexture)
        {
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, colourTexture);
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, normalTexture);
			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, positionTexture);
			GL.ActiveTexture(TextureUnit.Texture3);
			GL.BindTexture(TextureTarget.Texture2D, modelTexture);

			shader.LoadLight(Game.Instance.SunLight);
			shader.LoadCameraPosition(Game.Instance.MainCamera.gameObject.transform.GetWorldPosition());
			shader.LoadEnviromentMap(1);
			shader.LoadTextureUnits();
		}

		public void Unbind()
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.ActiveTexture(TextureUnit.Texture3);
			GL.BindTexture(TextureTarget.Texture2D, 0);

		}

		public void Render(FrameBuffer frameBuffer)
		{
			shader.Start();

			Bind(frameBuffer.ColourAttachmentIds[0].Value, 
				 frameBuffer.ColourAttachmentIds[1].Value, 
				 frameBuffer.ColourAttachmentIds[2].Value, 
				 frameBuffer.ColourAttachmentIds[3].Value);
			base.Render();
			Unbind();

			shader.Stop();
		}

		public void Dispose()
		{
			shader.Dispose();
		}

		private LightingShader shader;
	}
}
