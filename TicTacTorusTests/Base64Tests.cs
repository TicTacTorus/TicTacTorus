using NUnit.Framework;
using TicTacTorus.Source.Utility;

namespace TicTacTorusTests
{
	public class Tests
	{
		private Base64 id;
		[SetUp]
		public void Setup()
		{
			id = new Base64("UnitTest");
		}

		[Test]
		public void Test1()
		{
			Assert.AreEqual("UnitTest", id.ToString());
		}
	}
}