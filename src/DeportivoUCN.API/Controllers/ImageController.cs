using DeportivoUCN.Infrastructure.Data;
using DeportivoUCN.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeportivoUCN.API.Controllers;

[ApiController]
[Route("api/images")]
public class ImageController(DeportivoUCNContext context) : ControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageDto dto)
    {
        var file = dto.File;
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No se ha subido ningún archivo o el archivo está vacío" });
        }

        // Validar que el archivo sea una imagen
        if (!file.ContentType.StartsWith("image/"))
        {
            return BadRequest(new { message = "El archivo debe ser una imagen válida (JPG, PNG, GIF, SVG, etc.)" });
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var image = new UploadedImage
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                Data = memoryStream.ToArray()
            };

            context.UploadedImages.Add(image);
            await context.SaveChangesAsync();

            var imageUrl = $"{Request.Scheme}://{Request.Host}/api/images/{image.Id}";

            return Ok(new
            {
                message = "Imagen subida exitosamente",
                data = new
                {
                    id = image.Id,
                    url = imageUrl,
                    fileName = image.FileName,
                    contentType = image.ContentType,
                    uploadedAt = image.UploadedAt
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno al guardar la imagen", details = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetImage(Guid id)
    {
        var image = await context.UploadedImages.FirstOrDefaultAsync(img => img.Id == id);
        if (image == null)
        {
            return NotFound(new { message = "Imagen no encontrada" });
        }

        return File(image.Data, image.ContentType);
    }
}

public class UploadImageDto
{
    public required IFormFile File { get; set; }
}

