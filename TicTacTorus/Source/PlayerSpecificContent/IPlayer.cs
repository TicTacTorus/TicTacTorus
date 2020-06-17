using System.Drawing;
using Microsoft.AspNetCore.SignalR;
using TicTacTorus.Source.Ingame.GridSpecificContent.Grid;
using TicTacTorus.Source.Ingame.Move;
using TicTacTorus.Source.Persistence;

namespace TicTacTorus.Source.PlayerSpecificContent
{
    public interface IPlayer
    {
        #region Fields

        public string ID { get; set; }
        //public string SessionID { get; set; }
        public byte Index { get; set; }
        public string InGameName { get; set; }
        public Color PlrColor { get; set; }
        public byte Symbol { get; set; }
        public byte[] Salt { get; set; } 
        public byte[] Hash { get; set; }
        public PlayerStats playerStats { get; set; }// Besprechen?
        
        #endregion
        #region Methods

        IMove ChooseMove(IClientProxy connection, IGrid grid, int moveSeconds);

        #endregion
    }
}