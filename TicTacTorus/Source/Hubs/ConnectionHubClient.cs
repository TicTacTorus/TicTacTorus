using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic;
using TicTacTorus.Source.Utility;

namespace TicTacTorus.Source.Hubs
{
	public class ConnectionHubClient
	{
		#region Fields

		public readonly HubConnection Connection;

		#endregion
		#region Constructor

		public ConnectionHubClient(NavigationManager nav)
		{
			Connection = new HubConnectionBuilder()
				.WithUrl(nav.ToAbsoluteUri("/gameHub"))
				.Build();
			Connection.StartAsync();
		}

		#endregion
	}
}