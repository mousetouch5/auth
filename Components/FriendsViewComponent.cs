using Microsoft.AspNetCore.Mvc;

namespace auth.Components
{
    public class FriendsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
