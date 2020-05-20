using TicTacTorus.Source.Ingame.GridSpecificContent.Chunk;

namespace TicTacTorus.Source.Ingame.Referee
{
    public interface IReferee
    {
        bool HasWon(Grid grid, GlobalPos pos);
    }
}