using TicTacTorus.Source.Ingame.GridSpecificContent.Grid;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Ingame.Move
{
    public interface IMove
    {
        public byte Owner { set; get; }
        
        bool CanDo(IGrid grid, Permutation playerOrder);
        
        void Do(IGrid grid, Permutation playerOrder);

        void Undo(IGrid grid, Permutation playerOrder);
    }
}