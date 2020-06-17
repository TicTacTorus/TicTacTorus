using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Generator
{
    public class LobbyFactory
    {
        public static ILobby CreateLobbyWithId(IPlayer owner)
        {
            ILobby lobby = new Lobby(CreateLobbyName(owner), CreateLobbyStatus(),
                CreateLobbyDescription(owner), 20, true);
            AddId(lobby);
            return lobby;
        }

        public static ILobby CreateRandomLobbyWithId()
        {
            IPlayer owner = new HumanPlayer(AnonymPlayerNameGenerator.GetString(), ColorGenerator.GetColor(),
                ByteGenerator.GetByte());
            return CreateLobbyWithId(owner);
        }
        
        private static void AddId(ILobby l)
        {
            var id = Base64.Random();
            if (!Server.Instance.LobbyIdIsUnique(id.ToString())) AddId(l);
            l.Id = id;
        }

        private static string CreateLobbyName(IPlayer owner)
        {
            return owner.InGameName + "'s Game";
        }

        private static string CreateLobbyStatus()
        {
            return "just opened";
        }

        private static string CreateLobbyDescription(IPlayer owner)
        {
            return owner.InGameName + "'s Description";
        }
    }
}