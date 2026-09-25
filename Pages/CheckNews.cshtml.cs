using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VERITAS.Pages;

public class CheckNewsModel : PageModel
{
    [BindProperty]
    [Display(Name = "News headline")]
    [StringLength(500, ErrorMessage = "The headline cannot exceed 500 characters.")]
    public string? Headline { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Please enter a news claim.")]
    [StringLength(5000, ErrorMessage = "The claim cannot exceed 5000 characters.")]
    [Display(Name = "News content or claim")]
    public string? Claim { get; set; }

    [BindProperty]
    [Url(ErrorMessage = "Please enter a valid source URL.")]
    [StringLength(2000, ErrorMessage = "The source URL is too long.")]
    [Display(Name = "Source URL")]
    public string? SourceUrl { get; set; }

    public string? StatusMessage { get; set; }

    public void OnGet()
    {
    }

    public void OnPost()
    {
        if (!ModelState.IsValid)
        {
            return;
        }

        StatusMessage =
            "Your claim has been received. The verification engine will be connected in a later phase.";
    }
}