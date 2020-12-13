using OpenTK;
using Senapp.Engine.UI;

namespace Senapp.Engine
{
    public class Transform
    {
        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
        public Vector3 localScale { get; set; }

        public Matrix4 TransformationMatrix()
        {
            return CalculateTransformationMatrix();
        }
        public Matrix4 TransformationMatrixUI(Vector3 cameraPosition)
        {
            return CalculateTransformationMatrixUI(cameraPosition);
        }

        public static Transform NewWithScale(float x, float y, float z)
        {
            return new Transform(0, 0, 0, 0, 0, 0, x, y, z);
        }
        public static Transform NewWithScale(float x, float y)
        {
            return new Transform(0, 0, 0, 0, 0, 0, x, y, 0);
        }
        public static Transform NewFromRotation(float x, float y, float z)
        {
            return new Transform(0, 0, 0, x, y, z);
        }
        public static Transform NewFromRotation(float x, float y)
        {
            return new Transform(0, 0, 0, x, y, 0);
        }
        public Transform(): this(Vector3.Zero, Vector3.Zero, Vector3.One) { }
        public Transform(float x, float z) : this(x, 0, z) { }
        public Transform(float x , float y, float z) : this(new Vector3(x,y,z), Vector3.Zero, Vector3.One) { }
        public Transform(float x, float y, float z, float rotationFactor, float scaleFactor) : this(new Vector3(x, y, z), new Vector3(rotationFactor), new Vector3(scaleFactor)) { }
        public Transform(float xPosition, float yPosition, float zPosition, float xRotation, float yRotation, float zRotation) : this(new Vector3(xPosition, yPosition, zPosition), new Vector3(xRotation, yRotation, zRotation), Vector3.One) { }
        public Transform(float xPosition, float yPosition, float zPosition, float xRotation, float yRotation, float zRotation, float xScale, float yScale, float zScale) : this(new Vector3(xPosition, yPosition, zPosition), new Vector3(xRotation, yRotation,zRotation), new Vector3(xScale,yScale,zScale)) { }
        public Transform(bool UI, float xPosition, float yPosition, float xRotation, float yRotation, float xScale, float yScale) : this(new Vector3(xPosition, yPosition, 0), new Vector3(xRotation, yRotation, 0), new Vector3(xScale, yScale, 0)) { }
        public Transform(bool UI, float xPosition, float yPosition, float xRotation, float yRotation) : this(new Vector3(xPosition, yPosition, 0), new Vector3(xRotation, yRotation, 0), Vector3.One) { }

        public Transform(Vector3 position, Vector3 rotation, Vector3 localScale)
        {
            this.position = position;
            this.rotation = rotation;
            this.localScale = localScale;
        }
        public void Translate(Vector3 position, bool UI = false)
        {
            this.position += position;
        }
        public void Translate(float x, float y, float z)
        {
            Translate(new Vector3(x, y, z));
        }
        public void Translate(float x, float y)
        {
            Translate(new Vector3(x, y, 0));
        }
        public void Translate(float translationFactor)
        {
            Translate(new Vector3(translationFactor, translationFactor, translationFactor));
        }
        public void Rotate(float rotation)
        {
            Rotate(new Vector3(rotation, 0,0));
        }
        public void Rotate(float x, float y)
        {
            Rotate(new Vector3(x, y, 0));
        }
        public void Rotate(float x, float y, float z)
        {
            Rotate(new Vector3(x,y,z));
        }
        public void Rotate(Vector3 rotation)
        {
            this.rotation += rotation;
        }
        public void Scale(float scaleFactor)
        {
            Scale(new Vector3(scaleFactor, scaleFactor, scaleFactor));
        }
        public void Scale(float x, float y)
        {
            Scale(new Vector3(x, y, 1));
        }
        public void Scale(float x, float y, float z)
        {
            Scale(new Vector3(x, y, z));
        }
        public void Scale(Vector3 scaleFactor)
        {
            localScale += scaleFactor;
        }
        private Matrix4 CalculateTransformationMatrix()
        {
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, position.Z));
            Matrix4 rota = Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation.Y, rotation.Z, rotation.X));
            Matrix4 scale = Matrix4.CreateScale(new Vector3(localScale.X, localScale.Z, localScale.Y));
            Matrix4 transformationMatrix = Matrix4.Mult(rota, Matrix4.Mult(scale, translation));
            return transformationMatrix;
        }
        private Matrix4 CalculateTransformationMatrixUI(Vector3 cameraPosition)
        {  
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(cameraPosition.X+position.X,cameraPosition.Y + position.Y, cameraPosition.Z + position.Z));
            Matrix4 rota = Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation.Y, rotation.Z, rotation.X));
            Matrix4 scale = Matrix4.CreateScale(new Vector3(localScale.X, localScale.Z, localScale.Y));
            Matrix4 transformationMatrix = Matrix4.Mult(rota, Matrix4.Mult(scale, translation));
            return transformationMatrix;
        }
        public Transform TransformFromUIConstraint(UIConstraint constraint)
        {
            Transform val = new Transform();
            if (constraint == UIConstraint.None)
                val = this;
            else if (constraint == UIConstraint.TopLeft)
                val = new Transform(new Vector3(-Game.AspectRatio + (0.5f * this.localScale.X) + this.position.X, 1 - this.localScale.Y + this.localScale.Y / 2 + this.position.Y, -1), this.rotation, this.localScale);
            else if (constraint == UIConstraint.Top)
                val = new Transform(new Vector3(this.position.X, 1 - this.localScale.Y + this.localScale.Y / 2 + this.position.Y, -1), this.rotation, this.localScale);
            else if (constraint == UIConstraint.TopRight)
                val = new Transform(new Vector3(Game.AspectRatio - (0.5f * this.localScale.X) + this.position.X, 1 - this.localScale.Y + this.localScale.Y / 2 + this.position.Y, -1), this.rotation, this.localScale);
            else if (constraint == UIConstraint.CenterLeft)
                val = new Transform(new Vector3(-Game.AspectRatio + (0.5f * this.localScale.X) + this.position.X, this.position.Y, -1), this.rotation, this.localScale);
            else if (constraint == UIConstraint.Center)
                val = new Transform(new Vector3(this.position.X, this.position.Y, -1), this.rotation, this.localScale);
            else if (constraint == UIConstraint.CenterRight)
                val = new Transform(new Vector3(Game.AspectRatio - (0.5f * this.localScale.X) + this.position.X, this.position.Y, -1), this.rotation, this.localScale);
            else if (constraint == UIConstraint.BottomLeft)
                val = new Transform(new Vector3(-Game.AspectRatio + (0.5f * this.localScale.X) + this.position.X, -this.localScale.Y / this.localScale.Y + this.localScale.Y / 2 + this.position.Y, -1), this.rotation, this.localScale);
            else if (constraint == UIConstraint.Bottom)
                val = new Transform(new Vector3(this.position.X, -this.localScale.Y / this.localScale.Y + this.localScale.Y / 2 + this.position.Y, -1), this.rotation, this.localScale);
            else if (constraint == UIConstraint.BottomRight)
                val = new Transform(new Vector3(Game.AspectRatio - (0.5f * this.localScale.X) + this.position.X, -this.localScale.Y / this.localScale.Y + this.localScale.Y / 2 + this.position.Y, -1), this.rotation, this.localScale);
            return val;
        }
    }
}
