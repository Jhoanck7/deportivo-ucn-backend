using Microsoft.AspNetCore.Mvc;

namespace DeportivoUCN.API.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult TestConnection()
    {
        return Ok(new { mensaje = "¡Conexión exitosa con el backend de Deportivo UCN!" });
    }
}
