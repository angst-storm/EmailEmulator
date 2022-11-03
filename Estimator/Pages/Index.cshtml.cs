using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Estimator.Pages;

public class IndexModel : PageModel
{
    public void OnGet()
    {
        Program.Send100Mails();
    }
}