using System;
using System.ComponentModel.DataAnnotations;

namespace DXApplication1.Models
{
    public class ShapeInput
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Şekil türü zorunludur.")]
        public string ShapeType { get; set; }  // "Kare" veya "Dikdörtgen"

        [Required(ErrorMessage = "1. parametre zorunludur.")]
        [Display(Name = "1. Kenar")]
        public double Parameter1 { get; set; }  // Her şekil için zorunlu

        [Display(Name = "2. Kenar (sadece dikdörtgen için)")]
        public double? Parameter2 { get; set; }  // Dikdörtgen için gerekli, karede boş kalabilir

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsCalculated { get; set; }

        public string UserId { get; set; }
        public double? Area { get; set; }

    }
}
