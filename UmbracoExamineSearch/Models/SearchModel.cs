using System.ComponentModel.DataAnnotations;

namespace UmbracoExamineSearch.Models
{
    public class SearchModel : BaseModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "Maximum string length -> 200 characters!")]
        public string SearchText { get; set; }
    }
}