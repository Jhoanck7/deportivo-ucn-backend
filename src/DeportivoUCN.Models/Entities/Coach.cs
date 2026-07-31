namespace DeportivoUCN.Models.Entities;

public class Coach
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<SportBranch> SportBranches { get; set; } = new List<SportBranch>();
}