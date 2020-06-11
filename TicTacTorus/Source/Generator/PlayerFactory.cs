using TicTacTorus.Source.PlayerSpecificContent;

namespace TicTacTorus.Source.Generator
{
    public class PlayerFactory
    {
        public static HumanPlayer CreateHumanPlayer()
        {
            return new HumanPlayer(AnonymPlayerNameGenerator.GetString(), 
                ColorGenerator.GetColor(), ByteGenerator.GetByte());
        }
    }
}