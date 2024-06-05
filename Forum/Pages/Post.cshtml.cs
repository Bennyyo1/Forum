using Forum.Areas.Identity.Data;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Pages
{
    public class PostModel : PageModel
    {
        private readonly ForumContext _forumContext;
        private readonly UserManager<ForumUser> _userManager;
		private readonly BadWordFilter _badWordFilter;

		public Models.Post Post { get; set; }
        [BindProperty]
        public Models.Comment Comment { get; set; }
        public List<Models.Comment> Comments { get; set; }
        public Models.SubCategory SubCategory { get; set; }

        [BindProperty]
        public int? ReplyToCommentId { get; set; }

		[BindProperty]
		public IFormFile PostImage { get; set; }

		public PostModel(ForumContext forumContext, UserManager<ForumUser> userManager, BadWordFilter badWordFilter)
        {
            _forumContext = forumContext;
            _userManager = userManager;
			_badWordFilter = badWordFilter;
		}

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await LoadPageData(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string submitButton, int postId, int? commentId, int replyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            switch (submitButton)
            {
                case "AddComment":
                    Comment.Date = DateTime.Now;
                    Comment.UserId = user.Id;
                    Comment.PostId = postId;
					Comment.Text = _badWordFilter.Filter(Comment.Text);
					_forumContext.Comment.Add(Comment);
                    break;

                case "ShowReplyForm":
                    ReplyToCommentId = commentId; 
                    break;

                case "SubmitReply":
                    var replyText = Request.Form["ReplyText"];
                    var replyComment = new Comment
                    {
						Text = _badWordFilter.Filter(replyText),
						Date = DateTime.Now,
                        UserId = user.Id,
                        PostId = postId,
                        ParentCommentId = commentId
                    };
                    _forumContext.Comment.Add(replyComment);
                    ReplyToCommentId = null;
                    break;

                case "UpvotePost":
                    var post = await _forumContext.Post.FindAsync(postId);
                    if (post != null)
                    {
                        post.Votes++;
                        _forumContext.Post.Update(post);
                    }
                    break;

                case "DownvotePost":
                    post = await _forumContext.Post.FindAsync(postId);
                    if (post != null)
                    {
                        post.Votes--;
                        _forumContext.Post.Update(post);
                    }
                    break;

                case "UpvoteComment":
                    var comment = await _forumContext.Comment.FindAsync(commentId);
                    if (comment == null)
                    {
                        throw new ArgumentNullException(nameof(Comment), "Comment not found.");
                    }
                    comment.Votes++;
                    break;

                case "DownvoteComment":
                    comment = await _forumContext.Comment.FindAsync(commentId);
                    if (comment != null)
                    {

                        comment.Votes--;
                        _forumContext.Comment.Update(comment);

                        
                        postId = comment.PostId;
                    }
                    break;

                case "UpvoteReply":
					var reply = await _forumContext.Comment.FindAsync(replyId);
					if (reply != null)
					{
						reply.Votes++;
						_forumContext.Comment.Update(reply);
					}
					break;

				case "DownvoteReply":
					reply = await _forumContext.Comment.FindAsync(replyId);
					if (reply != null)
					{
						reply.Votes--;
						_forumContext.Comment.Update(reply);
					}
					break;

				case "ReportComment":
					comment = await _forumContext.Comment.FindAsync(commentId);
					if (comment != null)
					{
						comment.IsReported=true;
                        _forumContext.Comment.Update(comment);

						postId = comment.PostId;
					}
					break;

				case "ReportPost":
					 post = await _forumContext.Post.FindAsync(postId);
					if (post != null)
					{
						post.IsReported = true;
						_forumContext.Post.Update(post);
					}
					break;

				case "ReportReply":
					reply = await _forumContext.Comment.FindAsync(replyId);
					if (reply != null)
					{
						reply.IsReported = true;
						_forumContext.Comment.Update(reply);

					}
					
					break;


				default:
                    return BadRequest("Invalid action.");

					
			}

			if (PostImage != null)
			{
				var filePath = Path.Combine("wwwroot/postImages", PostImage.FileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await PostImage.CopyToAsync(stream);
				}
				Post.PostImagePath = PostImage.FileName;
			}

			await _forumContext.SaveChangesAsync();

            if (postId == 0)
            {
                throw new ArgumentNullException(nameof(Post), "Post not found.");
            }

            await LoadPageData(postId); // Reload data after saving changes
            return Page();
        }


        public async Task LoadPageData(int postId)
        {
            Post = await _forumContext.Post.Include(p => p.User).FirstOrDefaultAsync(m => m.Id == postId);

            if (Post == null)
            {
                throw new ArgumentNullException(nameof(Post), "Post not found.");
            }

            SubCategory = await _forumContext.SubCategory.FirstOrDefaultAsync(s => s.Id == Post.SubCategoryId);

            
            if (SubCategory == null)
            {
                throw new ArgumentNullException(nameof(SubCategory), "SubCategory not found.");
            }

            Comments = await _forumContext.Comment
                .Where(c => c.PostId == postId && c.ParentCommentId == null)
                .Include(c => c.User)
                .Include(c => c.Replies.OrderBy(r => r.Id)) 
                .ThenInclude(r => r.User)
                .ToListAsync();

            
        }

    }

}
