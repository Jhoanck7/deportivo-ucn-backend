using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DeportivoUCN.API.Controllers;

public class NewsItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
}

public class EventItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

[ApiController]
[Route("api")]
public class NewsEventController : ControllerBase
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "news_events.json");
    private static readonly object FileLock = new();

    private class StorageModel
    {
        public List<NewsItem> News { get; set; } = [];
        public List<EventItem> Events { get; set; } = [];
    }

    private StorageModel LoadData()
    {
        lock (FileLock)
        {
            if (!System.IO.File.Exists(FilePath))
            {
                var seed = new StorageModel
                {
                    News = new List<NewsItem>
                    {
                        new() { Id = 1, Title = "UCN clasifica a la final interuniversitaria", Content = "Nuestra selección de básquetbol varones derrotó a su rival histórico y clasificó a la gran final del torneo regional.", Category = "ÉXITO DEPORTIVO", ImageUrl = "https://images.unsplash.com/photo-1461896836934-ffe607ba8211?auto=format&fit=crop&w=600&q=80", Date = "2026-07-30" },
                        new() { Id = 2, Title = "Trabajos de mantención en Cancha Principal", Content = "Se han completado las labores anuales de cepillado y reposición de caucho en la cancha sintética.", Category = "INFRAESTRUCTURA", ImageUrl = "https://images.unsplash.com/photo-1545809074-59472b3f5eca?auto=format&fit=crop&w=600&q=80", Date = "2026-07-24" }
                    },
                    Events = new List<EventItem>
                    {
                        new() { Id = 1, Title = "Pruebas masivas de fútbol femenil", Description = "Asiste a las pruebas masivas para el equipo representativo de fútbol femenino de nuestra universidad.", Date = "2026-08-05", Location = "Cancha 1 - Pasto Sintético" },
                        new() { Id = 2, Title = "Clínica de Tenis de Mesa", Description = "Aprende técnicas avanzadas con entrenadores federados en una jornada abierta.", Date = "2026-08-12", Location = "Sala de Multiuso" }
                    }
                };
                var json = JsonSerializer.Serialize(seed, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(FilePath, json);
                return seed;
            }

            try
            {
                var json = System.IO.File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<StorageModel>(json) ?? new StorageModel();
            }
            catch
            {
                return new StorageModel();
            }
        }
    }

    private void SaveData(StorageModel data)
    {
        lock (FileLock)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(FilePath, json);
        }
    }

    [HttpGet("news")]
    public IActionResult GetNews()
    {
        var data = LoadData();
        return Ok(new { message = "News retrieved successfully", data = data.News });
    }

    [HttpPost("news")]
    public IActionResult CreateNews([FromBody] NewsItem item)
    {
        var data = LoadData();
        item.Id = data.News.Count > 0 ? data.News.Max(n => n.Id) + 1 : 1;
        if (string.IsNullOrEmpty(item.Date))
        {
            item.Date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        }
        data.News.Add(item);
        SaveData(data);
        return Created("", new { message = "News created successfully", data = item });
    }

    [HttpDelete("news/{id:int}")]
    public IActionResult DeleteNews(int id)
    {
        var data = LoadData();
        var item = data.News.FirstOrDefault(n => n.Id == id);
        if (item == null) return NotFound(new { message = "News item not found" });
        data.News.Remove(item);
        SaveData(data);
        return Ok(new { message = "News deleted successfully" });
    }

    [HttpGet("events")]
    public IActionResult GetEvents()
    {
        var data = LoadData();
        return Ok(new { message = "Events retrieved successfully", data = data.Events });
    }

    [HttpPost("events")]
    public IActionResult CreateEvent([FromBody] EventItem item)
    {
        var data = LoadData();
        item.Id = data.Events.Count > 0 ? data.Events.Max(e => e.Id) + 1 : 1;
        data.Events.Add(item);
        SaveData(data);
        return Created("", new { message = "Event created successfully", data = item });
    }

    [HttpDelete("events/{id:int}")]
    public IActionResult DeleteEvent(int id)
    {
        var data = LoadData();
        var item = data.Events.FirstOrDefault(e => e.Id == id);
        if (item == null) return NotFound(new { message = "Event not found" });
        data.Events.Remove(item);
        SaveData(data);
        return Ok(new { message = "Event deleted successfully" });
    }
}
