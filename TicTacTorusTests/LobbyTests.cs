using System.Drawing;
using NUnit.Framework;
using TicTacTorus.Source;
using TicTacTorus.Source.PlayerSpecificContent;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.ServerHandler;

namespace TicTacTorusTests
 
{
    public class LobbyTests
    {
        private  bool ins = false;
        private Lobby l;
   
        private HumanPlayer player;
        private LobbyList _lobbyList;
        [SetUp]
        public void Setup()
        {
     
            player = new HumanPlayer("namep","namei",Color.Aquamarine, 12,"namep");
            l = new Lobby("name", "offen", "offen", 2, false);
       
        }
        [Test]
        public void Test_LobbyNameIsUnique()
        {
            ins = Server.Instance.LobbyIdIsUnique("name");
            // ins = Server.Instance.AddLobby(l);
            Assert.AreEqual(true, ins);

        }

        [Test]
        public void Test_PlayersInGameAfterStart()
        {
          
        }
        [Test]
        public void Test_PlayerOrder()
        {
          
        }
        [Test]
        public void Test_GameStartIfPressButton()
        {
          
        }
    }
}