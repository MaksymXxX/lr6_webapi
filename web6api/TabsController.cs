using Microsoft.AspNetCore.Mvc;

namespace web6api
{
    [ApiController]
    [Route("api/tabs")]
    public class TabsController : ControllerBase
    {
        private const string DataFile = "tabs_storage.txt";

        [HttpPost("save")]
        public IActionResult SaveTabData([FromBody] TabsRequest request)
        {
            // Перевірка на правильність моделі
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Невірний формат даних" });
            }

            try
            {
                Console.WriteLine("Отримані дані:");
                foreach (var tab in request.Tabs)
                {
                    Console.WriteLine($"Entry: Heading={tab.Heading}, Details={tab.Details}");
                }

                using var writer = new StreamWriter(DataFile);
                foreach (var tab in request.Tabs)
                {
                    writer.WriteLine($"{tab.Heading}%%{tab.Details}");
                }

                return Ok(new { message = "Дані успішно збережено" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Сталася помилка: {ex.Message}" });
            }
        }

        [HttpGet("retrieve")]
        public IActionResult GetTabData()
        {
            try
            {
                if (!System.IO.File.Exists(DataFile))
                {
                    return Ok(new { entries = new List<Tab>() });
                }

                var lines = System.IO.File.ReadAllLines(DataFile);
                var tabs = lines.Select(line =>
                {
                    var parts = line.Split("%%");
                    return new Tab
                    {
                        Heading = parts[0],
                        Details = parts.Length > 1 ? parts[1] : string.Empty
                    };
                }).ToList();

                return Ok(new { entries = tabs });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Сталася помилка: {ex.Message}" });
            }
        }
    }

    public class TabsRequest
    {
        public List<Tab> Tabs { get; set; }
    }

    public class Tab
    {
        public string Heading { get; set; }
        public string Details { get; set; }

        public Tab() { }

        public Tab(string heading, string details)
        {
            Heading = heading;
            Details = details;
        }
    }
}
