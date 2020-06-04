using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.LobbySpecificContent
{
    public class Lobby : ILobby
    {
        [Required]
        [StringLength(10, ErrorMessage = "Description is too long.")] 
        public string Name { get; set; }
        public int MaxPlayerCount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        
        public Base64 Id { get; set; }
        public bool IsPrivate { get; set; }

        public List<IPlayer> Players { get; set; }

        public Lobby()
        {
            Players = new List<IPlayer>();
        }
        public Lobby(string name,int maxPlayerCount, bool isPrivate)
        {
            Players = new List<IPlayer>(maxPlayerCount);
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
            : this(name, status, description, maxPlayerCount, isPrivate)
        {
            Players.Add(owner);
        }
        public Lobby(Base64 id, string name, IPlayer owner ,string status, string description, int maxPlayerCount, bool isPrivate) 
            : this(name, owner, status, description, maxPlayerCount, isPrivate)
        {
            Id = id;
        }
        
        public bool AddPlayer(IPlayer player)
        {
            if (Players.Count < MaxPlayerCount)
            {
                Players.Add(player);
                return true;
            }

            return false;
        }

        public bool RemovePlayer(IPlayer player)
        {
            return Players.Remove(player);
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

        public IList<IPlayer> GetAllPlayers()
        {
            return Players;
        }
    }

    
}