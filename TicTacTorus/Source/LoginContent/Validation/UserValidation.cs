using System.ComponentModel.DataAnnotations;


namespace TicTacTorus.Source.LoginContent.Validation
{
	public class UserValidation
	{
		[Required]
		[StringLength(128, MinimumLength = 1, ErrorMessage = "Name should be between 1-128 characters long!")]
		public string Name { get; set; }
		
		[Required]
		[StringLength(9999, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long!")]
		public string Password { get; set; }
		
		[Required]
		[StringLength(9999, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long!")]
		public string PasswordConfirm { get; set; }
	}
}