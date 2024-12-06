using System.ComponentModel.DataAnnotations;

namespace SantanderAssessment.Controllers
{
    public class GetStoriesRequest
    {
        [Range(1, 200, ErrorMessage = "Limit must be between 1 and 200.")]
        public int Limit { get; set; } = 50; // Domyślna wartość
    }
}
