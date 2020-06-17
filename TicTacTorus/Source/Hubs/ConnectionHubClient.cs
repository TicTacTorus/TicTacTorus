using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
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

		public ConnectionHubClient(NavigationManager nav, string id = "")
		{
			Connection = new HubConnectionBuilder()
				.WithUrl(nav.ToAbsoluteUri("/connectionHub/" + id), HttpTransportType.WebSockets)
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