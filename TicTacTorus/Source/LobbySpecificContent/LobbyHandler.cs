using TicTacTorus.Source.Generator;
using TicTacTorus.Source.PlayerSpecificContent;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public static class LobbyHandler
    {
        public static ILobby CreateLobby(HumanPlayer player)
        {
            var server = Server.Instance;
            var lobby = LobbyFactory.CreateLobbyWithId(player);
            server.AddLobby(lobby);
            return lobby;
        }
        public static ILobby AddPlayerToLobby(string lobbyId, IPlayer player)
        {
            ILobby lobby = Server.Instance.GetLobbyById(lobbyId);
            
            lobby.Players.Add(player);
            
            return lobby;
        }
        
        
    }
}