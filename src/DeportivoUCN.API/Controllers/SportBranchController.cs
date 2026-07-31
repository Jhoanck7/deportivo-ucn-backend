using DeportivoUCN.Application.DTO;
using DeportivoUCN.Application.DTO.SportBranch;
using DeportivoUCN.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeportivoUCN.API.Controllers;

[ApiController]
[Route("api/sport-branches")]
public class SportBranchController(ISportBranchService sportBranchService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GenericResponse<IEnumerable<SportBranchResponseDto>>>> GetAll()
    {
        var response = await sportBranchService.GetAllBranchesAsync();
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GenericResponse<SportBranchResponseDto>>> GetById(int id)
    {
        var response = await sportBranchService.GetBranchByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<GenericResponse<SportBranchResponseDto>>> Create([FromBody] SportBranchRequestDto request)
    {
        var response = await sportBranchService.CreateBranchAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<GenericResponse<bool>>> Update(int id, [FromBody] SportBranchRequestDto request)
    {
        var response = await sportBranchService.UpdateBranchAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<GenericResponse<bool>>> Delete(int id)
    {
        var response = await sportBranchService.DeleteBranchAsync(id);
        return Ok(response);
    }
}