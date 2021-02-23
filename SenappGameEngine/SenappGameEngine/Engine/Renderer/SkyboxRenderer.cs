using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Senapp.Engine.Entities;
using Senapp.Engine.Models;
using Senapp.Engine.Shaders;

namespace Senapp.Engine.Renderer
{
    public class SkyboxRenderer
    {
		private static float SIZE = 500;
		public static int skyboxTextureID;


		private static float[] VERTICES = {
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

		private static string[] TEXTURES_FILES = { "right", "left", "top", "bottom", "front", "back" };
		private string start = "";
		private string ext = ".png";

		private RawModel cube;

		private SkyboxShader shader;
		public SkyboxRenderer(SkyboxShader _shader, Matrix4 projectionMatrix)
		{
			cube = Loader.LoadToVAO(VERTICES, 3);
			skyboxTextureID = Loader.LoadCubeMap(TEXTURES_FILES, start, ext);
			shader = _shader;
			shader.Start();
			shader.LoadProjectionMatrix(projectionMatrix);
			shader.Stop();
		}
		public void Render(Camera camera)
		{
			MasterRenderer.DisableCulling();
			shader.Start();
			shader.LoadViewMatrix(camera.GetViewMatrix());
			shader.LoadTransformationMatrix(camera.gameObject.transform.TransformationMatrixTranslation());
			GL.BindVertexArray(cube.vaoID);
			GL.EnableVertexAttribArray(0);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.TextureCubeMap, skyboxTextureID);
			GL.DrawArrays(PrimitiveType.Triangles, 0, cube.vertexCount);
			GL.DisableVertexAttribArray(0);
			GL.BindVertexArray(0);
			shader.Stop();
			MasterRenderer.EnableCulling();
		}
		public void CleanUp()
        {
			shader.CleanUp();
        }
	}
}
