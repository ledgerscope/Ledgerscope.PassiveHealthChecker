using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ledgerscope.PassiveHealthChecker.Sample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;

        [BindProperty]
        public string RequestText { get; set; }

        public IndexModel(ILogger<IndexModel> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, RequestText));
                // This is just a placeholder for a POST action.
                // You can implement any logic you need here.
                _logger.LogInformation("POST request received on Index page.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during POST request on Index page.");
                return RedirectToPage();
            }
            return RedirectToPage();
        }
    }
}
