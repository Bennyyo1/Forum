using Forum.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Post
    {
        public int Id { get; set; } //postens Id

        public string Title { get; set; } //postens titel

        public string TextContext { get; set; } //vad står i posten

        public int SubCategoryId { get; set; } //vilken subKategori tillhör posten (subKategori tillhör Kategori)

        public DateTime Date { get; set; } //när skrevs posten

        public bool IsReported { get; set; } //är den rapporterad?

        public string UserId { get; set; } // key till ForumUser

        public int? Votes { get; set; } = 1; //varje post börjar på +1

		public string? PostImagePath { get; set; }  

		[ForeignKey("UserId")]
        public virtual ForumUser User { get; set; } //navigation property

        public List<Comment> Comments { get; set; } 
    }
}
