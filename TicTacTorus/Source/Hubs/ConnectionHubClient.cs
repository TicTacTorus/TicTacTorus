using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

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
				.ConfigureLogging(logging => {
					logging.SetMinimumLevel(LogLevel.Information);
					logging.AddConsole();
				})
				.Build();
			Connection.StartAsync();
		}

		#endregion
	}
}