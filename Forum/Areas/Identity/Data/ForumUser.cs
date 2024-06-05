using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Forum.Models;
using Microsoft.AspNetCore.Identity;

namespace Forum.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ForumUser class
public class ForumUser : IdentityUser
{
    [PersonalData] //GDPR privacy
    
    public int Birthyear { get; set; }

    [PersonalData]
    public string FirstName { get; set; }

    [PersonalData]
    public string LastName { get; set; }

    public string NickName { get; set; }

    public string ProfilePicture { get; set; }

    public string? AboutMe { get; set; }

    public List<string> Messages { get; set; } = new List<string>();

    // Navigation property for posts created by the user
    public virtual ICollection<Post> Posts { get; set; }

}

