using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VERITAS.Pages
{
    public class ContactModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
        public string? Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter a subject.")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")]
        public string? Subject { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please enter your message.")]
        [StringLength(5000, ErrorMessage = "Message cannot exceed 5000 characters.")]
        public string? Message { get; set; }

        public bool MessageSent { get; private set; }

        public void OnGet()
        {
            MessageSent = false;
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                MessageSent = false;
                return;
            }

            MessageSent = true;

            Name = null;
            Email = null;
            Subject = null;
            Message = null;
        }
    }
}