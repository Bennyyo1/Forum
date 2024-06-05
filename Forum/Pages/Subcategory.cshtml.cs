using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Forum.Pages
{
    public class SubcategoryModel : PageModel
    {
        private readonly ForumContext _forumContext;
        public readonly UserManager<Areas.Identity.Data.ForumUser> _userManager;
        private readonly BadWordFilter _badWordFilter;

        [BindProperty]
        public Models.Post Post { get; set; } //lägga till ny post

        public List<SubCategory> SubCategories { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentIndex { get; set; } //subcategory.id = sidans index

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } // Subcategory ID from route


        [BindProperty]
        public IFormFile PostImage { get; set; }

        public Models.SubCategory SubCategory { get; set; }
        public List<Post> Posts { get; set; }
        public SubcategoryModel(ForumContext forumContext, UserManager<Areas.Identity.Data.ForumUser> userManager, BadWordFilter badWordFilter)
        {
            _forumContext = forumContext;
            _userManager = userManager;
            _badWordFilter = badWordFilter;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            SubCategory = await _forumContext.SubCategory.FirstOrDefaultAsync(sc => sc.Id == Id);

            Posts = await _forumContext.Post.Where(p => p.SubCategoryId == Id).ToListAsync(); //hämta bara poster med samma id som subCategory

            return Page(); //returnerar nuvarande sida
        }

        public async Task<IActionResult> OnPostAsync(string submitButton)
        {
            if (submitButton == "AddPost")
            {
                if (Post.SubCategoryId != 0)
                {
                    Post.Date = DateTime.Now;
                    Post.IsReported = false;

                    if (PostImage != null)
                    {
                        var filePath = Path.Combine("wwwroot/postImages", PostImage.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await PostImage.CopyToAsync(stream);
                        }
                        Post.PostImagePath = PostImage.FileName;
                    }

                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        Post.UserId = user.Id;
                        Post.Title = _badWordFilter.Filter(Post.Title); //Filter title
                        Post.TextContext = _badWordFilter.Filter(Post.TextContext); //Filter textarea

                        
                        _forumContext.Post.Add(Post);
                        await _forumContext.SaveChangesAsync();

                        
                        return RedirectToPage("./Subcategory", new { id = Post.SubCategoryId });
                    }
                    else
                    {
                        return BadRequest("User not found.");
                    }
                }
            }

            return Page();
        }

    }
}
