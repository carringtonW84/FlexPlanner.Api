namespace FlexPlanner.Api.DTOs
{
    public class UpdateUserRequest
    {
        public string? Password { get; set; }
        public Guid? TeamId { get; set; } // Gardé pour compatibilité
        public List<Guid>? TeamIds { get; set; } // Nouvelle propriété pour les équipes multiples
        public int? Velocity { get; set; }
    }
}
