
namespace DeportivoUCN.Application.DTO.SportBranch;

public class SportBranchRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string TrainingDays { get; set; } = string.Empty;
    public string TrainingHours { get; set; } = string.Empty;
    public string TrainingSector { get; set; } = string.Empty;
    public int AthleteLimit { get; set; }
    public int? CoachId { get; set; }
}