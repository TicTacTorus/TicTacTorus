using System.ComponentModel.DataAnnotations;

namespace TicTacTorus.Source.LoginContent.Validation
{
	public class UserRegisterValidation
	{
		[Required]
		[StringLength(256, ErrorMessage = "Login Name too long (max. 256 character)")]
		public string Id { get; set; }
		[Required]
		[StringLength(256, ErrorMessage = "In-Game Name too long (max. 256 character)")]
		public string InGameName { get; set; }
		[Required]
		[StringLength(256, ErrorMessage = "Password should be between 8 and 256 characters long", MinimumLength = 8)]
		public string Password { get; set; }
		[Required]
		public string ConfirmPassword { get; set; }
	}
}