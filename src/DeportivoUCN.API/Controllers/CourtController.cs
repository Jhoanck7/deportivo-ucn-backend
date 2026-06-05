using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.Court;
using DeportivoUCN.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeportivoUCN.API.Controllers;

[ApiController]
[Route("api/courts")]
public class CourtController(ICourtService courtService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GenericResponse<IEnumerable<CourtResponseDto>>>> GetAll()
    {
        var response = await courtService.GetAllCourtsAsync();
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GenericResponse<CourtResponseDto>>> GetById(int id)
    {
        var response = await courtService.GetCourtByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GenericResponse<CourtResponseDto>>> Create([FromBody] CourtRequestDto request)
    {
        var response = await courtService.CreateCourtAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GenericResponse<bool>>> Update(int id, [FromBody] CourtRequestDto request)
    {
        var response = await courtService.UpdateCourtAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GenericResponse<bool>>> Delete(int id)
    {
        var response = await courtService.DeleteCourtAsync(id);
        return Ok(response);
    }
}
