namespace FlexPlanner.Api.DTOs
{
    public class UpdateUserRequest
    {
        public string? Password { get; set; }
        public Guid? TeamId { get; set; }
        public int? Velocity { get; set; }
    }
}
