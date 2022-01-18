
namespace DM
{
    
    public interface IGameState
    {
        void PrepareLevel(Level level);
        void StartLevel(Level level);
        void ClearLevel();
    }
}
