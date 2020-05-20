﻿using System.Collections.Generic;
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
                {new Base64(), new MockLobbyList("Daniel's Cube World", 8, 10, "Waiting", "Cubes")},
                {new Base64(), new MockLobbyList("Tim's Sauna Landschaft", 2, 5, "Waiting", "No nerds plz")},
                {new Base64(), new MockLobbyList("Jack's DnD Dungeon", 12, 15, "Waiting", "Only DnD Fans!")}
            };
        }

        public string Name { get; set; }
        public int MaxPlayerCount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public Base64 Id { get; set; }
        public bool IsPrivate { get; set; }
        private IList<IPlayer> _players;
        
        public bool addPlayer(IPlayer player)
        {
            throw new System.NotImplementedException();
        }

        public IList<IPlayer> GetAllPlayers()
        {
            return _players;
        }
    }
}