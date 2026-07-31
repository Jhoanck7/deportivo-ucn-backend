namespace DeportivoUCN.Application.DTO.SportBranch;

public class SportBranchResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TrainingDays { get; set; } = string.Empty;
    public string TrainingHours { get; set; } = string.Empty;
    public string TrainingSector { get; set; } = string.Empty;
    public int AthleteLimit { get; set; }
    public int? CoachId { get; set; }
}