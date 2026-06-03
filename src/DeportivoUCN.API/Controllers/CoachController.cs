using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Coach;
using DeportivoUCN.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeportivoUCN.API.Controllers;

[ApiController]
[Route("api/coaches")]
public class CoachController(ICoachService coachService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GenericResponse<IEnumerable<CoachResponseDto>>>> GetAll()
    {
        var response = await coachService.GetAllCoachesAsync();
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GenericResponse<CoachResponseDto>>> GetById(int id)
    {
        var response = await coachService.GetCoachByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<GenericResponse<CoachResponseDto>>> Create([FromBody] CoachRequestDto request)
    {
        var response = await coachService.CreateCoachAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<GenericResponse<bool>>> Update(int id, [FromBody] CoachRequestDto request)
    {
        var response = await coachService.UpdateCoachAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<GenericResponse<bool>>> Delete(int id)
    {
        var response = await coachService.DeleteCoachAsync(id);
        return Ok(response);
    }
}