using dotnet_training.Data;
using dotnet_training.Models;
using Microsoft.EntityFrameworkCore;
using static dotnet_training.Helpers.Configs;

namespace dotnet_training.Services
{
    public class UrlShorteningService(AppDbContext _context)
    {
        private readonly Random _random = new();
        public async Task<string> GenerateUniqueCodeAsync()
        {
            var codeChars = new char[ShortLinkSettings.CodeLength];
            int maxValue = ShortLinkSettings.Alphabet.Length;

            while (true)
            {
                for (var i = 0; i < ShortLinkSettings.CodeLength; i++)
                {
                    var randomIndex = _random.Next(maxValue);

                    codeChars[i] = ShortLinkSettings.Alphabet[randomIndex];
                }

                var code = new string(codeChars);

                if (!await _context.ShortenerUrls.AnyAsync(s => s.Code == code))
                {
                    return code;
                }
            }
        }

        public async Task<ShortenerUrl> CreateAsync(ShortenerUrl shortenerUrl)
        {

            _context.ShortenerUrls.Add(shortenerUrl);
            await _context.SaveChangesAsync();

            return shortenerUrl;
        }

        public async Task<ShortenerUrl> FindByCodeAsync(string code)
        {
            return await _context.ShortenerUrls.FirstOrDefaultAsync(s => s.Code.Equals(code));
        }
    }
}
