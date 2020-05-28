﻿using System.Collections.Generic;
 using TicTacTorus.Source.PlayerSpecificContent;
 using TicTacTorus.Source.Utility;

 namespace TicTacTorus.Source.LobbySpecificContent
{
    public class MockLobbyList : ILobby
    {
        
        public MockLobbyList(string name, int currentCount, int maxCount, string status, string desc)
        {
            Name = name;
            MaxPlayerCount = maxCount;
            Status = status;
            Description = desc;
        }
        public static Dictionary<Base64, ILobby> GetAllLobbies()
        {
            return new Dictionary<Base64, ILobby>
            {
                {Base64.Random(), new MockLobbyList("Daniel's Cube World", 8, 10, "Waiting", "Cubes")},
                {Base64.Random(), new MockLobbyList("Tim's Sauna Landschaft", 2, 5, "Waiting", "No nerds plz")},
                {Base64.Random(), new MockLobbyList("Jack's DnD Dungeon", 12, 15, "Waiting", "Only DnD Fans!")}
            };
        }

        public string Name { get; set; }
        public int MaxPlayerCount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public Base64 Id { get; set; }
        public bool IsPrivate { get; set; }
        private IList<IPlayer> _players = new List<IPlayer>();
        
        public bool AddPlayer(IPlayer player)
        {
            throw new System.NotImplementedException();
        }

        public bool RemovePlayer(IPlayer player)
        {
            return _players.Remove(player);
        }

        public bool RemovePlayer(byte index)
        {
            throw new System.NotImplementedException();
        }

        public IPlayer GetPlayerAt(byte index)
        {
            return _players[index];
        }

        public IList<IPlayer> GetAllPlayers()
        {
            return _players;
        }
    }
}