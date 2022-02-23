using OpenTK;
using OpenTK.Graphics.OpenGL4;

using Senapp.Engine.Entities;
using Senapp.Engine.Loaders;
using Senapp.Engine.Models;
using Senapp.Engine.Shaders;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Renderer
{
    public class SkyboxRenderer
    {
		private const float SIZE = 500;	
		public static int SkyboxTextureID;

		public SkyboxRenderer(Matrix4 projectionMatrix)
		{
			skyboxPrefix = Settings.GetSetting<string>(ConfigSettings.SKYBOX_FILE_PREFIX);
			cube = Loader.LoadPositionsToVAO(VERTICES, 3, skyboxPrefix + "_SKYBOX");
			SkyboxTextureID = Loader.LoadCubeMap(TEXTURES_FILES, skyboxPrefix);

			shader = new SkyboxShader();

			shader.Start();
			shader.LoadProjectionMatrix(projectionMatrix);
			shader.Stop();
		}

		public void Render(bool isColourPass, Camera camera)
		{
			MasterRenderer.DisableCulling();

			shader.Start();

			shader.LoadViewMatrix(camera.GetViewMatrix());
			shader.LoadTransformationMatrix(camera.gameObject.transform.TransformationMatrixTranslation());
			shader.LoadIsColourPass(isColourPass);

			GL.BindVertexArray(cube.VaoId);
			GL.EnableVertexAttribArray(0);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.TextureCubeMap, SkyboxTextureID);
			GL.DrawArrays(PrimitiveType.Triangles, 0, cube.VertexCount);
			GL.DisableVertexAttribArray(0);
			GL.BindVertexArray(0);

			shader.Stop();

			MasterRenderer.EnableCulling();
		}

		public void Dispose()
        {
			shader.Dispose();
        }
	
		private readonly RawModel cube;
		private readonly SkyboxShader shader;

		private readonly string skyboxPrefix = "";
		private static readonly string[] TEXTURES_FILES = { "right", "left", "top", "bottom", "front", "back" };
		private static readonly float[] VERTICES = {
			-SIZE,  SIZE, -SIZE,
			-SIZE, -SIZE, -SIZE,
			 SIZE, -SIZE, -SIZE,
			 SIZE, -SIZE, -SIZE,
			 SIZE,  SIZE, -SIZE,
			-SIZE,  SIZE, -SIZE,

			-SIZE, -SIZE,  SIZE,
			-SIZE, -SIZE, -SIZE,
			-SIZE,  SIZE, -SIZE,
			-SIZE,  SIZE, -SIZE,
			-SIZE,  SIZE,  SIZE,
			-SIZE, -SIZE,  SIZE,

			 SIZE, -SIZE, -SIZE,
			 SIZE, -SIZE,  SIZE,
			 SIZE,  SIZE,  SIZE,
			 SIZE,  SIZE,  SIZE,
			 SIZE,  SIZE, -SIZE,
			 SIZE, -SIZE, -SIZE,

			-SIZE, -SIZE,  SIZE,
			-SIZE,  SIZE,  SIZE,
			 SIZE,  SIZE,  SIZE,
			 SIZE,  SIZE,  SIZE,
			 SIZE, -SIZE,  SIZE,
			-SIZE, -SIZE,  SIZE,

			-SIZE,  SIZE, -SIZE,
			 SIZE,  SIZE, -SIZE,
			 SIZE,  SIZE,  SIZE,
			 SIZE,  SIZE,  SIZE,
			-SIZE,  SIZE,  SIZE,
			-SIZE,  SIZE, -SIZE,

			-SIZE, -SIZE, -SIZE,
			-SIZE, -SIZE,  SIZE,
			 SIZE, -SIZE, -SIZE,
			 SIZE, -SIZE, -SIZE,
			-SIZE, -SIZE,  SIZE,
			 SIZE, -SIZE,  SIZE
		};
	}
}
