using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

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
				.WithUrl(nav.ToAbsoluteUri("/connectionHub"))
				.Build();
			Connection.StartAsync();
		}

		#endregion
	}
}