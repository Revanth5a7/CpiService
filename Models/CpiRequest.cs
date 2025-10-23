namespace CpiService.Models
{
    public class CpiRequest
    {
        public int Year { get; set; }
        public string Month { get; set; } = string.Empty;
    }
}