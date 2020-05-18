﻿

using System.Collections.Generic;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public class Lobby : ILobby
    {
        public string Name { get; set; }
        public int MaxPlayerCount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public bool IsPrivate { get; set; }

        private IList<IPlayer> _players;

        public Lobby()
        {
            _players = new List<IPlayer>();
        }
        public Lobby(string name,int maxPlayerCount, bool isPrivate)
        {
            _players = new List<IPlayer>(maxPlayerCount);
            MaxPlayerCount = maxPlayerCount;
            Name = name;
            IsPrivate = isPrivate;
        }
        public Lobby(string name, string status, string description, int maxPlayerCount, bool isPrivate)
        {
            _players = new List<IPlayer>(maxPlayerCount);
            MaxPlayerCount = maxPlayerCount;
            Name = name;
            IsPrivate = isPrivate;
            Status = status;
            Description = description;
        }
        public bool addPlayer(IPlayer player)
        {
            if (_players.Count < MaxPlayerCount)
            {
                _players.Add(player);
                return true;
            }

            return false;
        }

        public IList<IPlayer> GetAllPlayers()
        {
            return _players;
        }

    }

    
}