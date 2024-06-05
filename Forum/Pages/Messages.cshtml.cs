using System.Collections.Generic;
using System.Threading.Tasks;
using Forum.Areas.Identity.Data;
using Forum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Forum.Pages
{
    public class MessagesModel : PageModel
    {
        private readonly ForumContext _context;
        private readonly UserManager<ForumUser> _userManager;

        public IList<string> UserMessages { get; set; } //användarens meddelanden

        public MessagesModel(ForumContext context, UserManager<ForumUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //HARDCODE MSG FOR TEST
            //user.Messages ??= new List<string>();
            //user.Messages.Add("This is a test message.");
            //var result = await _userManager.UpdateAsync(user);

            await LoadUserMessages(user);
            return Page();
        }
        private async Task LoadUserMessages(ForumUser user)
        {
            
            UserMessages = user.Messages; //hämta användarens meddelanden
        }

        public async Task<IActionResult> OnPostSendMessage(string nicknames, string message)
        {
            if (string.IsNullOrEmpty(nicknames))
            {
                ModelState.AddModelError("nicknames", "Please enter at least one nickname.");
                return Page();
            }

            var nicknameList = nicknames.Split(','); 

            foreach (var nickname in nicknameList)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NickName == nickname);
                if (user != null)
                {

                    
                    user.Messages ??= new List<string>();
                    user.Messages.Add(message);
                    await _userManager.UpdateAsync(user);
                   
                }
                else
                {
                    ModelState.AddModelError("nicknames", $"User not found.");
                }
            }

            return RedirectToPage("/Messages");
        }

        public async Task<IActionResult> OnPostDeleteMessage(string message)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            user.Messages.Remove(message);
            await _userManager.UpdateAsync(user);

            return RedirectToPage("/Messages");
        }
    }
}
