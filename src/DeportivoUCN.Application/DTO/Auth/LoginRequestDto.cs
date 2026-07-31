namespace DeportivoUCN.Application.DTO.Auth;

public class LoginRequestDto
{
    public string EmailOrRut { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
