namespace ComputeApi.Models
{
    public class ShapeInputCalcDto
    {
        public int Id { get; set; }
        public string ShapeType { get; set; }
        public double Parameter1 { get; set; }
        public double? Parameter2 { get; set; }
        public string UserId { get; set; }
    }
}
