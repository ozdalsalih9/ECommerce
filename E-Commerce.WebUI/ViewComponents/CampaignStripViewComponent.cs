using Microsoft.AspNetCore.Mvc;
using E_Commerce.Service.Abstract;
using E_Commerse.Core.Entities;

namespace E_Commerse.WebUI.ViewComponents
{
    public class CampaignStripViewComponent : ViewComponent
    {
        private readonly IService<News> _newsService;

        public CampaignStripViewComponent(IService<News> newsService)
        {
            _newsService = newsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var newsList = await _newsService.GetAllAsync();
            return View("_CampaignStrip", newsList);
        }
    }
}
