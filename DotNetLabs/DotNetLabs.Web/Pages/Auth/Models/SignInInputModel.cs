using System.ComponentModel.DataAnnotations;

namespace DotNetLabs.Web.Pages.Auth;

public class SignInInputModel
{
    [Required]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
