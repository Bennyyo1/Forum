using Forum.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Comment
    {
        public int Id { get; set; } //Comments Id

        public string Text { get; set; } //Content of the comment

        public int PostId { get; set; } //Id of the post to which the comment belongs

        public DateTime Date { get; set; } 

        public string UserId { get; set; } //Key to ForumUser

        public int? ParentCommentId { get; set; }

        public bool IsReported { get; set; } //är den rapporterad?

        public int? Votes { get; set; } = 1; //varje post börjar på +1

        [ForeignKey("UserId")]
        public virtual ForumUser User { get; set; } //Navigation property

        [ForeignKey("PostId")]
        public virtual Post Post { get; set; } //Navigation prop

        [ForeignKey("ParentCommentId")]
        public virtual Comment ParentComment { get; set; }

        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>(); 
    }
}
