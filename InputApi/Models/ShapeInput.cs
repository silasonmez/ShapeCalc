using System;

namespace InputApi.Models
{
    public class ShapeInput
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string ShapeType { get; set; }
        public double Parameter1 { get; set; }
        public double? Parameter2 { get; set; }

        public double? Area { get; set; }
    
        public bool IsCalculated { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
