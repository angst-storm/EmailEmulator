using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Estimator.Pages;

public class IndexModel : PageModel
{
    public RedirectResult OnPostSend(int count)
    {
        Program.SendMails(count);
        return Redirect("/");
    }
}