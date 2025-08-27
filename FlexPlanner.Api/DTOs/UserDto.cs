namespace FlexPlanner.Api.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Velocity { get; set; }

        // Équipe principale (pour compatibilité ascendante)
        public TeamDto? Team { get; set; }

        // Liste de toutes les équipes (nouvelle fonctionnalité)
        public List<TeamDto> Teams { get; set; } = new();
    }
}
