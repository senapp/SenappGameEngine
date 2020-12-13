﻿using OpenTK;
using Senapp.Engine.Entities;

namespace Senapp.Engine.Shaders
{
    public class EntityShader : ShaderProgram
    {
        private static readonly string VERTEX_SHADER_FILE = "Resources/Shaders/vertexShader.glsl";
        private static readonly string FRAGMENT_SHADER_FILE = "Resources/Shaders/fragmentShader.glsl";

        private int location_transformationMatrix;
        private int location_projectionMatrix;
        private int location_viewMatrix;
        private int location_lightPosition;
        private int location_lightColour;
        private int location_shineDamper;
        private int location_reflectivity;
        private int location_luminosity;
        private int location_useFakeLighting;




        public EntityShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
            base.BindAttribute(1, "textureCoords");
            base.BindAttribute(2, "normal");

        }

        protected override void GetAllUniformLocations()
        {
            location_transformationMatrix = base.GetUniformLocation("transformationMatrix");
            location_projectionMatrix = base.GetUniformLocation("projectionMatrix");
            location_viewMatrix = base.GetUniformLocation("viewMatrix");
            location_lightPosition = base.GetUniformLocation("lightPosition");
            location_lightColour = base.GetUniformLocation("lightColour");
            location_shineDamper = base.GetUniformLocation("shineDamper");
            location_reflectivity = base.GetUniformLocation("reflectivity");
            location_luminosity = base.GetUniformLocation("luminosity");
            location_useFakeLighting = base.GetUniformLocation("useFakeLighting");
        }
        public void LoadUseFakeLightingVariable(bool useFake)
        {
            base.LoadBoolean(location_useFakeLighting, useFake);
        }
        public void LoadShineVariables(float damper, float reflectivity, float luminosity)
        {
            base.LoadFloat(location_shineDamper, damper);
            base.LoadFloat(location_reflectivity, reflectivity);
            base.LoadFloat(location_luminosity, luminosity);
        }
        public void LoadTransformationMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_transformationMatrix, matrix);
        }
        public void LoadProjectionMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_projectionMatrix, matrix);
        }
        public void LoadViewMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_viewMatrix, matrix);
        }
        public void LoadLight(Light light, Transform transform)
        {
            base.LoadVector(location_lightPosition, transform.position);
            base.LoadVector(location_lightColour, light.colour);

        }

        public void UpdateCamera(Camera camera, Transform transform)
        {
            LoadProjectionMatrix(camera.GetProjectionMatrix());
            LoadViewMatrix(camera.GetViewMatrix(transform));
        }
    }
}