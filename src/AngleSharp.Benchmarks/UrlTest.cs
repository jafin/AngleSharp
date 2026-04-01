using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AngleSharp.Benchmarks
{
    public sealed class UrlTest
    {
        private static readonly HttpClient http = new();

        private UrlTest(string name, string source)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Name = name;
            Source = source;
        }

        internal static async Task<UrlTest> For(string url, string extension, bool withBuffer)
        {
            try
            {
                var source = string.Empty;
                var uri = new Uri(url);
                var name = uri.Host.Replace("www.", "").Replace(".com", "").Replace(".de", "").Replace(".org", "");
                if (!Directory.Exists("temp"))
                {
                    Directory.CreateDirectory("temp");
                }
                var fileName = Path.Combine("temp", name + extension);

                if (!withBuffer || !File.Exists(fileName))
                {
                    const int maxRetries = 3;
                    const int minValidLength = 128;

                    for (var attempt = 1; attempt <= maxRetries; attempt++)
                    {
                        http.DefaultRequestHeaders.UserAgent.Clear();
                        http.DefaultRequestHeaders.UserAgent.ParseAdd(
                            "Mozilla/5.0 (compatible; TestBrowser/1.0; +https://github.com/anglesharp)");
                        var content = await http.GetAsync(uri);
                        source = await content.Content.ReadAsStringAsync();

                        if (source.Length >= minValidLength)
                            break;

                        Console.WriteLine("Attempt {0}/{1} for \"{2}\" returned only {3} chars, retrying...",
                            attempt, maxRetries, url, source.Length);

                        if (attempt < maxRetries)
                            await Task.Delay(1000 * attempt);
                    }

                    if (source.Length < minValidLength)
                        Console.WriteLine("Warning: \"{0}\" returned only {1} chars after {2} attempts",
                            url, source.Length, maxRetries);

                    if (withBuffer && source.Length >= minValidLength)
                    {
                        await File.WriteAllTextAsync(fileName, source);
                    }
                }
                else
                {
                    source = await File.ReadAllTextAsync(fileName);
                }

                return new UrlTest(name, source);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading \"{0}\": {1}", url, ex.Message);
                return null;
            }
        }

        public string Name { get; }

        public string Source { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}