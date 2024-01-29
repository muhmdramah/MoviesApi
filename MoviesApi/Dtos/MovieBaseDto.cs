namespace MoviesApi.Dtos
{
    public class MovieBaseDto
    {
        [MaxLength(250)]
        public string Title { get; set; }
        public int ProductionYear { get; set; }
        public double Rate { get; set; }
        [MaxLength(2500)]
        public string Storeline { get; set; }
        public byte GenreId { get; set; }
    }
}
