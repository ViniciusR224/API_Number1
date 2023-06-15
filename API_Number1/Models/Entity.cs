using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace API_Number1.Models
{
    public abstract class Entity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }=new Guid();
        [Required]
        public string Name { get; set; }
        
        

        
    }
}
