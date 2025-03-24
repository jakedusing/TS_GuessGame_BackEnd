using System;
using System.Web;
using Microsoft.AspNetCore.Components.Web;

public class SongService
{
    private static readonly HttpClient client = new HttpClient();
    private static Random random = new Random();
    
    private static readonly string[] songs = new[]
    {
        "Anti-Hero",
        "Cruel Summer",
        "Don't Blame Me",
        "Shake It Off",
        "Blank Space"
    };

    // Get random song and its lyrics
    public async Task<Song> GetRandomSongAsync()
    {
        // pick a random song from the list
        string songTitle = songs[random.Next(songs.Length)];

        // get lyrics for the song
        string lyrics = await GetLyricsAsync(songTitle);

        return new Song { Title = songTitle, Lyrics = lyrics };
    }

    // helper method to get the lyrics
    private async Task<string> GetLyricsAsync(string songTitle)
    {
        try
        {
            // URL encode the song title to ensure spaces are replaced with %20
            string encodedSongTitle = HttpUtility.UrlEncode(songTitle);

            encodedSongTitle = encodedSongTitle.Replace("+", "%20");

            // Construct the url to fetch lyrics
            var url = $"https://api.lyrics.ovh/v1/Taylor%20Swift/{encodedSongTitle}";

            Console.WriteLine("Requesting lyrics from URL: " + url);

            // Make HTTP request
            var response = await client.GetStringAsync(url);

            // If the response is empty, it means we recieved a 404 or some other error
            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Received an empty response. The song may not be found.");
                return "Lyrics not available";
            }

            // parse the response and extract lyrics
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

            // If lyrics are available, return them, otherwise return a fallback message
            if (data?.lyrics != null)
            {
                return data.lyrics;
            }
            else
            {
                // print full response for debugging
                Console.WriteLine("API Response: " + response);
                return "Lyrics not available.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return $"Error retrieving lyrics: {ex.Message}";
        }
    }
}