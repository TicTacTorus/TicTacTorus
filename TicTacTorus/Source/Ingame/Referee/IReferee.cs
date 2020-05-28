using TicTacTorus.Source.Ingame.GridSpecificContent.Grid;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;

namespace TicTacTorus.Source.Ingame.Referee
{
    public interface IReferee
    {
        bool HasWon(IGrid grid, GlobalPos pos);
    }
}