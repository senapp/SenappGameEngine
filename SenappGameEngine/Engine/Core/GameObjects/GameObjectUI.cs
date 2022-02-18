namespace Senapp.Engine.Core.GameObjects
{
    public class GameObjectUI : GameObject
    {
        public GameObjectUI(): base()
        {
            IsGameObjectUI = true;
            IsGameObjectUpdated = true;
        }
    }
}
