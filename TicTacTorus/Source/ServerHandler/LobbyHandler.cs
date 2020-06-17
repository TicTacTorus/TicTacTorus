using System;
using System.Collections.Generic;
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
			
			if (player.ID == null && !lobby.Players.Exists(p => p.InGameName == player.InGameName))
			{
				lobby.AddPlayer(player);
			}
			else if(!lobby.Players.Exists(p => p.ID == player.ID))
			{
				lobby.AddPlayer(player);
			}
			// prüfe per index
			if ( (lobby.Players.Count > player.Index) && ( (lobby.Players[player.Index] == null) ) )
			{
				lobby.AddPlayer(player);
			}
			
			return lobby;
		}

		public static List<IPlayer> RemovePlayerFromLobby(string lobbyId, IPlayer player)
		{
			var lobby = Server.Instance.GetLobbyById(lobbyId);
			lobby.RemovePlayer(player.Index);	// remove Player from Lobby (set null)

			// remove Lobby from Server if necessary
			if (lobby.PlayerCount <= 0)
			{
				Server.Instance.RemoveLobby(lobbyId);
			}

			return lobby.Players;
		}
        
        
	}
}