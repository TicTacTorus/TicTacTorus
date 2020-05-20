﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public class Lobby : ILobby
    {
        public string Name { get; set; }
        public int MaxPlayerCount { get; set; }
        public string Status { get; set; }
        
        [Required]
        [StringLength(10, ErrorMessage = "Description is too long.")]
        public string Description { get; set; }
        public Base64 Id { get; set; }
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
            : this(name, maxPlayerCount, isPrivate)
        {
            Status = status;
            Description = description;
        }
        public Lobby(string name, IPlayer owner ,string status, string description, int maxPlayerCount, bool isPrivate) 
            : this(name,status, description, maxPlayerCount, isPrivate)
        {
            _players.Add(owner);
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