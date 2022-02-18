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

		public SkyboxRenderer(SkyboxShader shader, Matrix4 projectionMatrix)
		{
			skyboxPrefix = Settings.GetSetting<string>(ConfigSettings.SKYBOX_FILE_PREFIX);
			cube = Loader.LoadPositionsToVAO(VERTICES, 3, skyboxPrefix + "_SKYBOX");
			SkyboxTextureID = Loader.LoadCubeMap(TEXTURES_FILES, skyboxPrefix);

			skyboxShader = shader;

			skyboxShader.Start();
			skyboxShader.LoadProjectionMatrix(projectionMatrix);
			skyboxShader.Stop();
		}

		public void Render(Camera camera)
		{
			MasterRenderer.DisableCulling();

			skyboxShader.Start();
			skyboxShader.LoadViewMatrix(camera.GetViewMatrix());
			skyboxShader.LoadTransformationMatrix(camera.gameObject.transform.TransformationMatrixTranslation());

			GL.BindVertexArray(cube.VaoId);
			GL.EnableVertexAttribArray(0);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.TextureCubeMap, SkyboxTextureID);
			GL.DrawArrays(PrimitiveType.Triangles, 0, cube.VertexCount);
			GL.DisableVertexAttribArray(0);
			GL.BindVertexArray(0);

			skyboxShader.Stop();

			MasterRenderer.EnableCulling();
		}

		public void Dispose()
        {
			skyboxShader.Dispose();
        }
	
		private readonly RawModel cube;
		private readonly SkyboxShader skyboxShader;

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
