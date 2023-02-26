namespace FlyMessenger.MVVM.Model
{
    public class ResponseValidationException
    {
        public int? Code { get; set; }
        public Details[] Details { get; set; }
    }
    
    public class Details
    {
        public string? Location { get; set; }
        public string? Field { get; set; }
        public string? Translation { get; set; }
        public string? Message { get; set; }
    }
}
