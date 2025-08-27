namespace FlexPlanner.Api.DTOs
{
    public class TeamCheckboxDto
    {
        public string Value { get; set; } = string.Empty; // Team ID
        public string Label { get; set; } = string.Empty; // Team Name
        public bool Checked { get; set; } = false; // Si l'utilisateur appartient à cette équipe
    }
}
