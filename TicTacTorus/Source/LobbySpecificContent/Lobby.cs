using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using TicTacTorus.Source.Ingame;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public class Lobby : ILobby
    {
        #region Data

        [Required]
        [StringLength(10, ErrorMessage = "Description is too long.")] 
        public string Name { get; set; }
        public int MaxPlayerCount { get; set; }
        public int PlayerCount { get; set; }

        public string Status { get; set; }
        public string Description { get; set; }
        
        public Base64 Id { get; set; }
        public bool IsPrivate { get; set; }

        public List<IPlayer> Players { get; set; }
        
        public GameSettings Settings { private set; get; }

        #endregion
        #region Constructors

        public Lobby()
        {
            Players = new List<IPlayer>();
        }
        public Lobby(string name, int maxPlayerCount, bool isPrivate)
        {
            Players = new List<IPlayer>(maxPlayerCount);
            MaxPlayerCount = maxPlayerCount;
            Name = name;
            IsPrivate = isPrivate;

            //todo give us sensible config values
            Settings = new GameSettings
            {
                GridSize = 50,
                WinChainLength = 5,
                TimeLimitSec = 0
            };
        }
        public Lobby(string name, string status, string description, int maxPlayerCount, bool isPrivate) 
            : this(name, maxPlayerCount, isPrivate)
        {
            Status = status;
            Description = description;
        }
        public Lobby(Base64 id, string name, string status, string description, int maxPlayerCount, bool isPrivate) 
            : this(name, status, description, maxPlayerCount, isPrivate)
        {
            Id = id;
        }
        
        public Lobby(string name, IPlayer owner ,string status, string description, int maxPlayerCount, bool isPrivate) 
            : this(name, status, description, maxPlayerCount, isPrivate)
        {
            Players.Add(owner);
        }
        public Lobby(Base64 id, string name, IPlayer owner ,string status, string description, int maxPlayerCount, bool isPrivate) 
            : this(name, owner, status, description, maxPlayerCount, isPrivate)
        {
            Id = id;
        }

        #endregion
        #region AccessMethods
        public bool AddPlayer(IPlayer player)
        {
            if (Players.Count >= MaxPlayerCount) return false;
            player.Index = (byte) Players.Count;
            Players.Add(player);
            PlayerCount++;
            return true;

        }

        /*
        public bool RemovePlayer(IPlayer player)
        {
            //return Players.Remove(player);
            byte indx = GetIndexByPlayer(player);
            if (indx >= 0xFF)
            {
                return false;
            }

            return RemovePlayer(indx);
        }
        
        public bool RemovePlayer(byte index)
        {
            if (Players.Count >= index)
            {
                return false;
            }
            Players.RemoveAt(index);
            return true;
        }
        */

        public void RemovePlayer(byte index)
        {
            if (Players[index] == null)
                return;
            Players[index] = null;
            PlayerCount--;
        }

        public IPlayer GetPlayerAt(byte index)
        {
            return Players[index];
        }

        public IPlayer GetPlayerById(string playerID)
        {
            foreach (var ePlayer in Players)
            {
                if (ePlayer.ID.Equals(playerID))
                {
                    return ePlayer;
                }
            }

            return null;
        }
        /*
        public byte GetIndexByPlayer(IPlayer player)
        {
            for(byte i=0; i<Players.Count; i++)
            {
                if (Players[i].Equals(player)) return i;
            }

            return 0xFF;
        }*/

        public List<IPlayer> GetAllPlayers()
        {
            return Players;
        }

        #endregion
    }

    
}