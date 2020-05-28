namespace TicTacTorus.Source.Generator
{
    public class PlayerFactory
    {
        public static HumanPlayer CreateHumanPlayer()
        {
            HumanPlayer hp = new HumanPlayer(AnonymPlayerNameGenerator.GetString(), ColorGenerator.GetColor(), ByteGenerator.GetByte());

            return hp;
        }
    }
}