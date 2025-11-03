
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Domain.Entities
{
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = default!;

        [Required]
        [Range(1950, 2100)]
        public int Year { get; set; } = default!;
    }
}