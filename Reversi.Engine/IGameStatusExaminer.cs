namespace Reversi.Engine
{
    public interface IGameStatusExaminer
    {
        GameStatus DetermineGameStatus(IGameContext context);
    }
}