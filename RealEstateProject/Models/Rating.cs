namespace RealEstateProject.Models
{
    public class Rating
    {
        public int RatingId { get; set; }

        public int? GiverId { get; set; }
        public User? Giver { get; set; }

        public int? ReceiverId { get; set; }
        public User? Receiver { get; set; }

        public int? Score { get; set; }

        public string Comment { get; set; } = string.Empty;
    }
}
