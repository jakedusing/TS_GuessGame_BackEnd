using System;
using System.Web;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json.Linq;

public class SongService
{
    private readonly IConfiguration _configuration;
    private static readonly HttpClient client = new HttpClient();

    public SongService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Song> GetLyricsAsync(string songTitle)
    {
        try {
            string uid = _configuration.GetValue<string>("API:UID");
            string token = _configuration.GetValue<string>("API:TOKEN");

            string encodedSongTitle = Uri.EscapeDataString(songTitle);
            string encodedArtist = Uri.EscapeDataString("Taylor Swift");

            string apiUrl = $"https://www.stands4.com/services/v2/lyrics.php?uid={uid}&tokenid={token}&term={encodedSongTitle}&artist={encodedArtist}&format=json";

            Console.WriteLine($"Fetching song data from: {apiUrl}");

            // Get API response
            string response = await client.GetStringAsync(apiUrl);
            JObject json = JObject.Parse(response);

            // Find the correct song entry for "Taylor Swift"
            var songEntry = json["result"]?
            .FirstOrDefault(entry => entry["artist"]?.ToString() == "Taylor Swift");

            if (songEntry == null)
            {
                Console.WriteLine("Song not found for Taylor Swift");
                return null;
            }

            string songLink = songEntry["song-link"]?.ToString();
            if (string.IsNullOrEmpty(songLink))
            {
                Console.WriteLine("No song link found.");
                return null;
            }

            Console.WriteLine($"Found song link: {songLink}");

            // Step 2: Scrape lyrics from lyrics.com page
            string lyrics = await ScrapeLyricsFromLyricsCom(songLink);

            if (string.IsNullOrEmpty(lyrics))
            {
                Console.WriteLine("No lyrics found");
                return null;
            }

            // Create and return a Song object with title and lyrics
            return new Song
            {
                Title = songTitle,
                Lyrics = lyrics
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    private async Task<string> ScrapeLyricsFromLyricsCom(string songUrl)
    {
        try
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(songUrl);

            // Lyrics are in a <pre> with class "lyric-body"
            var lyricsNode = doc.DocumentNode.SelectSingleNode("//pre | //div[@class='lyric-body']");

            if (lyricsNode == null)
            {
                Console.WriteLine("Lyrics not found on page.");
                return "Lyrics not available";
            }

            string lyrics = lyricsNode.InnerText.Trim();
            return lyrics.Replace("\r", "").Replace("\n", " ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error scrapping lyrics: {ex.Message}");
            return "Error retrieving lyrics.";
        }
    }
}