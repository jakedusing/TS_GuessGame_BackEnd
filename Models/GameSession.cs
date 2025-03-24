using System;

public class GameSession
{
    private Song song;
    private int revealCount;
    private int maxGuesses = 5;
    private int currentStartIndex;  // to track where lyrics start

    public int RemainingGuesses { get; private set; } = 5;
    public bool IsGameOver => RemainingGuesses <= 0;

    public GameSession(Song song)
    {
        this.song = song;
        revealCount = 5; // start with 3 words
        currentStartIndex = new Random().Next(0, song.Lyrics.Split(' ').Length - 1);
    }

    public string GetHint()
    {
        var cleanedLyrics = song.Lyrics.Replace("\n", " ");
        var words = cleanedLyrics.Split(' ');
        int maxWordsToReveal = Math.Min(revealCount, words.Length - currentStartIndex);
        return string.Join(" ", words[currentStartIndex..(currentStartIndex + maxWordsToReveal)]);
    }

    public object CheckGuess(string guess)
    {
        if (guess.Equals(song.Title, StringComparison.OrdinalIgnoreCase))
        {
            return new { correct = true, message = "Correct!", songTitle = song.Title };
        }
        else
        {
            RemainingGuesses--;
            revealCount += 2; // Reveal 2 more words
            return new
            {
                correct = false,
                message = "Wrong guess!",
                hint = GetHint(),
                RemainingGuesses = RemainingGuesses,
                gameOver = IsGameOver
            };
        }
    }
}