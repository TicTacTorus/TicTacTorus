
using NUnit.Framework;
using TicTacTorus.Source;
using TicTacTorus.Source.LobbySpecificContent;
using TicTacTorus.Source.Persistence;
using TicTacTorus.Source.Utility;
using System.Data.SQLite;
using System.Drawing;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
    using TicTacTorus.Source.Hubs;
    using TicTacTorus.Source.PlayerSpecificContent;


namespace TicTacTorusTests
{
    public class AccountTests
    {
    
       
        private bool res;
   
        [SetUp]
        public void Setup()
        {
            
        }
        [Test]
        public void Test_PlayerIdIsUnique()
        {
          res=  PersistenceStorage.CheckPlayerIdIsUnique("w1");
            

            Assert.AreEqual("false",res );

        }
        [Test]
        public void Test_Registration()
        {
            
        }
        [Test]
        public void Test_Login()
        {
            
        }
        [Test]
        public void Test_EditOwnUser()
        {
            
        }
        [Test]
        public void Test_ShowProfileDataCorectly()
        {
            
        }
        [Test]
        public void Test_InviteInGroup()
        {
            
        }  
        [Test]
        public void Test_ResetOwnStatistic()
        {
            
        }
        [Test]
        public void Test_DeleteOwnStatistic()
        {
            
        }  
        [Test]
        public void Test_ConnectionWebsite()
        {
            
        }
        [Test]
        public void Test_EditOwnProfileOptions()
        {
            
        }
        [Test]
        public void Test_LogoutUserCantEditProfile()
        {
            
        }
        [Test]
        public void Test_UserCantEditOtherProfile()
        {
            
        }
    }
}