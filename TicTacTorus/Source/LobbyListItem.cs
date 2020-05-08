using System.Collections;
using System.Collections.Generic;

namespace TicTacTorus.Source
{
	public class LobbyListItem
	{
		public string Name { get; private set; }
		public int PlayerCount { get; private set; }
		public int MaxPlayerCount { get; private set; }
		public string Status { get; private set; }
		public string Description { get; private set; }

		public LobbyListItem(string name, int currentCount, int maxCount, string status, string desc)
		{
			Name = name;
			PlayerCount = currentCount;
			MaxPlayerCount = maxCount;
			Status = status;
			Description = desc;
		}

		public static IList<LobbyListItem> GetAllLobbies()
		{
			IList<LobbyListItem> test = new List<LobbyListItem>();
			test.Add(new LobbyListItem("Daniel's Cube World", 8, 10, "Waiting", "Cubes"));
			test.Add(new LobbyListItem("Tim's Sauna Landschaft", 2, 5, "Waiting", "No nerds plz"));
			test.Add(new LobbyListItem("Jack's DnD Dungeon", 12, 15, "Waiting", "Only DnD Fans!"));
			return test;
		}
	}
}