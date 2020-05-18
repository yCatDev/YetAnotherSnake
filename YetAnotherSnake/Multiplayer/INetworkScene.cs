using Microsoft.Xna.Framework;

namespace YetAnotherSnake.Multiplayer
{
    public interface INetworkScene
    {
        bool IsReady { get; set; }
        void SetSnakePosition(int id, NetworkVector receivedSnakeMarkerPosition, float delta);
        void Pause();
        void UnPause();
        void CreateFood(Vector2 foodPos, Vector2 stonePos);
    }
}