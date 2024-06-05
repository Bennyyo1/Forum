namespace Forum.Models
{
    public class SubCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CategoryId { get; set; } //vilken överKategori tillhör den

		public List<Post>? Posts { get; set; } //Poster i denna subkategori
	}
}
