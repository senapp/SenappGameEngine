using OpenTK;

namespace Senapp.Engine.Physics
{
    public static class PhysicsExtensions
    {
        public class DirectionState
        {
            public Direction X;
            public Direction Y;
            public Direction Z;

            public DirectionState(Direction X = Direction.None, Direction Y = Direction.None, Direction Z = Direction.None)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }
        }
        public enum Direction
        {
            None,
            Negative,
            Positive
        }
        public static DirectionState GetDirectionState(Vector3 direction)
        {
            var X = direction.X < 0 ? Direction.Negative : direction.X == 0 ? Direction.None : Direction.Positive;
            var Y = direction.Y < 0 ? Direction.Negative : direction.Y == 0 ? Direction.None : Direction.Positive;
            var Z = direction.Z < 0 ? Direction.Negative : direction.Z == 0 ? Direction.None : Direction.Positive;

            return new DirectionState(X, Y, Z);
        }
    }
}
