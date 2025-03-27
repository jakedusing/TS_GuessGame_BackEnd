using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/game")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly SongService _songService;
    private static Dictionary<string, GameSession> activeSessions = new Dictionary<string, GameSession>();

    public GameController(SongService songService)
    {
        _songService = songService;
    }

    private static List<string> songTitles = new List<string>
    {
        "Lover", "Shake It Off", "Blank Space",

    };

    private static Random random = new Random();

    [HttpGet("start")]
    public async Task<IActionResult> StartGame()
    {
        var randomSongTitle = songTitles[random.Next(songTitles.Count)];
        var song = await _songService.GetLyricsAsync(randomSongTitle);
        string sessionId = Guid.NewGuid().ToString();
        activeSessions[sessionId] = new GameSession(song);

        return Ok(new { sessionId, hint = activeSessions[sessionId].GetHint() });
    }

    [HttpPost("guess")]
    public IActionResult MakeGuess([FromBody] GuessRequest request)
    {
        if (!activeSessions.ContainsKey(request.SessionId))
            return NotFound("Session not found.");

        var session = activeSessions[request.SessionId];
        var result = session.CheckGuess(request.Guess);
        
        return Ok(result);
    }
}