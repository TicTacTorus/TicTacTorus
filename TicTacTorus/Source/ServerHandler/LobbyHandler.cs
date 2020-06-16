using System;
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
			var lobby = Server.Instance.GetLobbyById(lobbyId);

			if (player.ID == null && !Server.Instance.Lobbies[lobbyId].Players.Exists(p => p.InGameName == player.InGameName))
			{
				lobby.Players.Add(player);
			}
			else if(!Server.Instance.Lobbies[lobbyId].Players.Exists(p => p.ID == player.ID))
			{
				lobby.Players.Add(player);
			}
			
			return lobby;
		}

		public static Tuple<bool, ILobby> RemovePlayerFromLobby(string lobbyId, IPlayer player)
		{
			ILobby lobby = Server.Instance.GetLobbyById(lobbyId);

			return Tuple.Create(lobby.RemovePlayer(player), lobby );
		}
        
        
	}
}