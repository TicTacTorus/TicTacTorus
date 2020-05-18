﻿using System.Collections.Generic;

namespace TicTacTorus.Source
{
    public class MockLobbyList
    {
        public string Name { get; private set; }
        public int PlayerCount { get; private set; }
        public int MaxPlayerCount { get; private set; }
        public string Status { get; private set; }
        public string Description { get; private set; }
		
        public string Id { get; }

        public MockLobbyList(string name, int currentCount, int maxCount, string status, string desc)
        {
            Name = name;
            PlayerCount = currentCount;
            MaxPlayerCount = maxCount;
            Status = status;
            Description = desc;
            Id = "ioRuZr82";
        }
        public static IEnumerable<MockLobbyList> GetAllLobbies()
        {
            IList<MockLobbyList> test = new List<MockLobbyList>();
            test.Add(new MockLobbyList("Daniel's Cube World", 8, 10, "Waiting", "Cubes"));
            test.Add(new MockLobbyList("Tim's Sauna Landschaft", 2, 5, "Waiting", "No nerds plz"));
            test.Add(new MockLobbyList("Jack's DnD Dungeon", 12, 15, "Waiting", "Only DnD Fans!"));
            return test;
        }
    }
}