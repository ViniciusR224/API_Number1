using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Number1.Models
{
    public class User:Entity
    {

        
        [Required]
        public string Email { get; set; }
        
        public Category Category { get; set; }
        [Required]
        [ForeignKey("Category")]
        public Guid CategoryId { get;set; }//Notação, o type da propriedade deve ser do mesmo do id da class extrageira

        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string PasswordSalt { get; set; }

        


    }
}
