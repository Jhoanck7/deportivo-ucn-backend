using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Athlete;
using DeportivoUCN.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeportivoUCN.API.Controllers;

[ApiController]
[Route("api/athletes")]
public class AthleteController(IAthleteService athleteService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GenericResponse<IEnumerable<AthleteResponseDto>>>> GetAll()
    {
        var response = await athleteService.GetAllAthletesAsync();
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GenericResponse<AthleteResponseDto>>> GetById(int id)
    {
        var response = await athleteService.GetAthleteByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<GenericResponse<AthleteResponseDto>>> Create([FromBody] AthleteRequestDto request)
    {
        var response = await athleteService.CreateAthleteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<GenericResponse<bool>>> Update(int id, [FromBody] AthleteRequestDto request)
    {
        var response = await athleteService.UpdateAthleteAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<GenericResponse<bool>>> Delete(int id)
    {
        var response = await athleteService.DeleteAthleteAsync(id);
        return Ok(response);
    }
}
