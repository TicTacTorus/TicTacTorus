using TicTacTorus.Source.Ingame.GridSpecificContent.Grid;
using TicTacTorus.Source.Ingame.GridSpecificContent.Position;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Ingame.Move
{
    public class PlayerShuffleMove : IMove
    {
        public byte Owner { set; get; }
        public Permutation Change { get; }

        public PlayerShuffleMove(byte owner, Permutation shuffle)
        {
            Owner = owner;
            Change = shuffle;
        }

        public PlayerShuffleMove(byte owner, params int[] shuffle) : this(owner, new Permutation(shuffle))
        {
        }
        
        public bool CanDo(IGrid grid, Permutation playerOrder)
        {
            return true;
        }

        public void Do(IGrid grid, Permutation playerOrder)
        {
            Change.Permute(playerOrder);
        }

        public void Undo(IGrid grid, Permutation playerOrder)
        {
            Change.Inverse().Permute(playerOrder);
        }


        public GlobalPos GetAreaCorner()
        {
            return new GlobalPos();
        }

        public int GetAreaWidth()
        {
            return 0;
        }

        public int GetAreaHeight()
        {
            return 0;
        }
    }
}