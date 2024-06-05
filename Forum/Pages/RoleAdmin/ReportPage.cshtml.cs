using Forum.Areas.Identity.Data;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Forum.Pages.RoleAdmin
{

	public class ReportPageModel : PageModel
	{

		private readonly ForumContext _forumContext;
		private readonly UserManager<ForumUser> _userManager;

		[BindProperty]
		public List<Models.Comment> Comments { get; set; }
		[BindProperty]
		public List<Models.Post> Posts { get; set; }

		public ReportPageModel(ForumContext forumContext, UserManager<ForumUser> userManager)
		{
			_forumContext = forumContext;
			_userManager = userManager;
		}
		public async Task<IActionResult> OnGetAsync()
		{
			
			Comments = await _forumContext.Comment.Where(c => c.IsReported).Include(c => c.User).ToListAsync(); //skapa lista av alla reportade kommentarer
			Posts = await _forumContext.Post.Where(p => p.IsReported).Include(p => p.User).ToListAsync();

			return Page();
		}

		//POSTS
		public async Task<IActionResult> OnPostDeletePost(int postId)
		{
			var postToDelete = await _forumContext.Post.FindAsync(postId);

			if (postToDelete != null)
			{
				_forumContext.Post.Remove(postToDelete);

				await _forumContext.SaveChangesAsync();
			}

			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostUnreportPost(int postId)
		{
			var postToUnreport = await _forumContext.Post.FindAsync(postId);

			if (postToUnreport != null)
			{
				postToUnreport.IsReported = false;

				await _forumContext.SaveChangesAsync();
			}

			return RedirectToPage();
		}


		//COMMENTS
		public async Task<IActionResult> OnPostDeleteComment(int commentId)
		{
			var commentToDelete = await _forumContext.Comment.FindAsync(commentId);

			if (commentToDelete != null)
			{
				
				var commentsToDelete = await _forumContext.Comment .Where(c => c.Id == commentId || c.ParentCommentId == commentId).ToListAsync();  //ta bort alla replies som ligger under kommentaren. RemoveRange respekterar FK?


				_forumContext.Comment.RemoveRange(commentsToDelete);

				
				await _forumContext.SaveChangesAsync();
			}

			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostUnreportComment(int commentId)
		{
			var commentToUnreport = await _forumContext.Comment.FindAsync(commentId);

			if (commentToUnreport != null)
			{

				commentToUnreport.IsReported = false;


				await _forumContext.SaveChangesAsync();
			}

			return RedirectToPage();
		}


	}
}
