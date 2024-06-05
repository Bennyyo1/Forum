using Forum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Forum.Pages.RoleAdmin
{
    public class IndexModel : PageModel
    {

        //ROLES USERS
        public List<Areas.Identity.Data.ForumUser> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }

        public readonly UserManager<Areas.Identity.Data.ForumUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

		private readonly ForumContext _forumContext;
		private readonly BadWordFilter _badWordFilter;

		//CATEGORIES & LISTS
		[BindProperty]
		public Models.Category Category { get; set; } //för lägga till ny huvudKategori
		public List<Models.Category> Categories { get; set; } //skapa en lista av alla Kategorier(Model.categories)

		[BindProperty]
		public Models.SubCategory SubCategory { get; set; } //för lägga till ny subKategori
		public List<Models.SubCategory> SubCategories { get; set; }//skapa en lista av alla SubKategorier(Model.SubCategories)

        [BindProperty]
        public Models.Post Post { get; set; } //lägga till ny post
		public List<Models.Post> Posts { get; set; }//skapa en lista av alla poster(Model.Posts)



		[BindProperty(SupportsGet =true)]
        public string RoleName { get; set; }

		[BindProperty(SupportsGet = true)]
		public string AddUserId { get; set; }

		[BindProperty(SupportsGet = true)]
		public string RemoveUserId { get; set; }

        public IndexModel(RoleManager<IdentityRole> roleManager, UserManager<Areas.Identity.Data.ForumUser> userManager, ForumContext forumContext, BadWordFilter badWordFilter)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _forumContext = forumContext;
			_badWordFilter = badWordFilter;
		}

        public async Task OnGetAsync(int deleteCategoryId, int deleteSubCategoryId)
        {

			if (deleteCategoryId != 0)
			{
				Models.Category categoryToBeDeleted = await _forumContext.Category.FindAsync(deleteCategoryId);

				if (categoryToBeDeleted != null)
				{

					_forumContext.Category.Remove(categoryToBeDeleted);
					await _forumContext.SaveChangesAsync();

				}

			}

			if (deleteSubCategoryId != 0)
			{
				Models.SubCategory subCategoryToBeDeleted = await _forumContext.SubCategory.FindAsync(deleteSubCategoryId);

				if (subCategoryToBeDeleted != null)
				{
					_forumContext.SubCategory.Remove(subCategoryToBeDeleted);
					await _forumContext.SaveChangesAsync();
				}
			}


			Roles = await _roleManager.Roles.ToListAsync();
            Users = await _userManager.Users.ToListAsync();

			Categories = await _forumContext.Category.ToListAsync(); //vid sidhämtning lägg kategori från DB till lista
			SubCategories = await _forumContext.SubCategory.ToListAsync();
			Posts = await _forumContext.Post.ToListAsync();

			if (AddUserId != null) 
            {
                var alterUser = await _userManager.FindByIdAsync(AddUserId); //hitta rätt användare
                await _userManager.AddToRoleAsync(alterUser, RoleName); //tilldela användaren ny roll(skickar med användarId och roll)
            }
            if(RemoveUserId != null)
            {
                var alterUser = await _userManager.FindByIdAsync(RemoveUserId);
                await _userManager.RemoveFromRoleAsync(alterUser, RoleName);
            }

        }

        public async Task<IActionResult> OnPostAsync(string submitButton)
		{
			switch (submitButton)
			{
				case "AddCategory":
					
					_forumContext.Category.Add(Category);
					break;

				case "AddSubcategory":
					if (SubCategory.CategoryId != 0)
					{
						
						_forumContext.SubCategory.Add(SubCategory);
					}
					break;

				case "AddPost":
					if (Post.SubCategoryId != 0)
					{
						Post.Date = DateTime.Now;
						Post.IsReported = false;

						var user = await _userManager.GetUserAsync(User);
						if (user != null)
						{
							Post.UserId = user.Id;
							Post.Title = _badWordFilter.Filter(Post.Title); //filter title
							Post.TextContext = _badWordFilter.Filter(Post.TextContext);//filter textarea
							_forumContext.Post.Add(Post);
						}
						else
						{
							return BadRequest("User not found.");
						}
					}
					break;

				case "NewRole":
					await CreateRole(RoleName);
					break;


				default:
					return BadRequest("Invalid action.");
			}

			await _forumContext.SaveChangesAsync();
			return RedirectToPage("Index");
		}

		public async Task CreateRole(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);

            if(!roleExists) //om roll inte finns skapa nytt objekt
            {
                var Role = new IdentityRole()
                {
                    Name = roleName
                };
                await _roleManager.CreateAsync(Role);
            }
        }
    }
}
