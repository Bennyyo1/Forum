using Forum.Areas.Identity.Data;
using Forum.Data;
using Forum.Migrations;
using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;

namespace Forum.Pages
{
    public class IndexModel : PageModel
    {

        private  UserManager<Areas.Identity.Data.ForumUser> _userManager {  get; set; }
        private readonly ForumContext _forumContext;
		private readonly IHttpClientFactory _clientFactory; //best practise för API?

		public Areas.Identity.Data.ForumUser MyUser { get; set; }


		//CATEGORIES & LISTS
		[BindProperty]
		public Models.Category Category { get; set; } //för lägga till ny huvudKategori
		public List<Models.Category> Categories { get; set; } //skapa en lista av alla Kategorier(Model.categories)

		[BindProperty]
		public Models.SubCategory SubCategory { get; set; } //för lägga till ny subKategori
		public List<Models.SubCategory> SubCategories { get; set; }//skapa en lista av alla SubKategorier(Model.SubCategories)

        public string ContentToFilter { get; set; }


        public IndexModel(UserManager<Areas.Identity.Data.ForumUser> userManager, ForumContext forumContext, IHttpClientFactory clientFactory) //CTOR
        {
            _userManager = userManager;
            _forumContext = forumContext;
			_clientFactory = clientFactory;
		}

        public async Task OnGetAsync()
        {

            MyUser = await _userManager.GetUserAsync(User);
            Categories = await GetCategoriesAPI();
			SubCategories = await _forumContext.SubCategory.ToListAsync();
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _forumContext.Category.Add(Category);
            await _forumContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<List<Category>> GetCategoriesAPI()
        {
			var httpClient = _clientFactory.CreateClient();
            var response = await httpClient.GetAsync("https://forumtoazureapi.azurewebsites.net/api/category");//PORT FRÅN API

            if (response.IsSuccessStatusCode)
            {
                var categoriesJson = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<Category>>(categoriesJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // JSON matchar inte med class prop annars....
                });

                return categories;
            }
            else
            {
				return new List<Category>(); //return tom list 
			}

			
		}



    }
}
