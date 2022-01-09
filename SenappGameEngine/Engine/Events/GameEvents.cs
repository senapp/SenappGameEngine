using System;

namespace Senapp.Engine.Events
{
    public delegate void GameInitializedEventHandler(object sender);
    public delegate void GameUpdatedEventHandler(object sender, GameUpdatedEventArgs args);
    public delegate void GameRenderedEventHandler(object sender, GameRenderedEventArgs args);
    public delegate void GameClosedEventHandler(object sender);
    public delegate void GameResizeEventHandler(object sender);

    public class GameUpdatedEventArgs : EventArgs
    {
        public readonly float DeltaTime;
        public GameUpdatedEventArgs(float deltaTime) : base()
        {
            DeltaTime = deltaTime;
        }
    }
    public class GameRenderedEventArgs : EventArgs
    {
        public readonly float DeltaTime;
        public GameRenderedEventArgs(float deltaTime) : base()
        {
            DeltaTime = deltaTime;
        }
    }

}
