using System.ComponentModel.DataAnnotations;

namespace TicTacTorus.Source.LoginContent.Validation
{
	public class UserLoginValidation
	{
		[Required]
		public string Id { get; set; }
		[Required]
		public string Password { get; set; }
	}
}