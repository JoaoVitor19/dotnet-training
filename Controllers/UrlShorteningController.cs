using dotnet_training.Data;
using dotnet_training.Models;
using dotnet_training.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_training.Controllers
{
    [Route("[controller]")]
    public class UrlShorteningController : Controller
    {
        private UrlShorteningService _urlShorteningService;

        public UrlShorteningController(AppDbContext context)
        {
            _urlShorteningService = new UrlShorteningService(context);
        }

        public record ShortenUrlRequest(string Url);

        [HttpPost]
        public async Task<ActionResult<string>> UrlShortening(ShortenUrlRequest shortenUrlRequest)
        {
            if (!Uri.TryCreate(shortenUrlRequest.Url, UriKind.Absolute, out _))
            {
                return BadRequest("The specified URL is invalid.");
            }

            var code = await _urlShorteningService.GenerateUniqueCodeAsync();

            var shortenedUrl = new ShortenerUrl
            {
                Id = Guid.NewGuid(),
                LongUrl = shortenUrlRequest.Url,
                Code = code,
                ShortUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/UrlShortening/{code}",
                CreatedOnUtc = DateTime.UtcNow
            };

            await _urlShorteningService.CreateAsync(shortenedUrl);

            return Ok(shortenedUrl.ShortUrl);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<string>> UrlShortening(string code)
        {
            var shortenedUrl = await _urlShorteningService.FindByCodeAsync(code);

            if (shortenedUrl is null)
                return NotFound();

            return Redirect(shortenedUrl.LongUrl);
        }
    }
}
