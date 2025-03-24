using Microsoft.AspNetCore.Mvc;

namespace auth.Components
{
    public class ChatbotViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
