using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/game")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly SongService songService = new SongService();
    private static Dictionary<string, GameSession> activeSessions = new Dictionary<string, GameSession>();

    [HttpGet("start")]
    public async Task<IActionResult> StartGame()
    {
        var song = await songService.GetRandomSongAsync();
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