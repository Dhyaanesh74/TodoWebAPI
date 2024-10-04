using System.ComponentModel.DataAnnotations;


namespace todowebapi.Models
{
    public class Todo{
        [Key]
        public int Id { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Type type { get; set; }

        public DateTime CreatedTime { get; set; } 

        public DateTime? Completed { get; set; }

        public Todo()
       {
        CreatedTime = DateTime.UtcNow;
       }

    }
    public enum Type {Personal, Work , other}
}