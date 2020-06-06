using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Generator
{
    public class LobbyFactory
    {
        public static Lobby CreateLobbyNoId(IPlayer owner)
        {
            return new Lobby(CreateLobbyName(owner), owner, CreateLobbyStatus(),
                CreateLobbyDescription(owner), 5, true);
        }
        
        public static ILobby CreateLobbyWithId(IPlayer owner)
        {
            ILobby lobby = new Lobby(CreateLobbyName(owner), owner, CreateLobbyStatus(),
                CreateLobbyDescription(owner), 5, true);
            lobby = AddId(lobby);
            return lobby;
        }

        public static ILobby CreateRandomLobbyWithId()
        {
            IPlayer owner = new HumanPlayer(AnonymPlayerNameGenerator.GetString(), ColorGenerator.GetColor(),
                ByteGenerator.GetByte());
            return CreateLobbyWithId(owner);
        }

        private static ILobby AddId(ILobby l)
        {
            var id = Base64.Random();
            if (Server.Instance.LobbyIdIsUnique(id.ToString()))
            {
                l.Id = id;
                return l;
            }
            return AddId(l);
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