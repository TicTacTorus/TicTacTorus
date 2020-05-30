using System.Drawing;
using Microsoft.AspNetCore.SignalR.Client;

namespace TicTacTorus.Source.PlayerSpecificContent
{
    public interface IPlayer
    {
        public string ID { get; set; }
        public string IngameName { get; set; }
        public Color Color { get; set; }
        public byte Symbol { get; set; }
    }
}