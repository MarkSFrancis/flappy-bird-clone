using System;

public static class Score
{
    public static void Setup()
    {
        Bird.Instance.OnDie += Bird_OnDie;
    }

    private static void Bird_OnDie(object sender, EventArgs e)
    {
        TrySetHighScore(Level.Instance.TotalPipesPassed);
    }

    public static int HighScore
    {
        get => SavedData.HighScore;
        set => SavedData.HighScore = value;
    }

    public static bool TrySetHighScore(int newScore)
    {
        if (HighScore >= newScore)
        {
            return false;
        }

        HighScore = newScore;
        return true;
    }

    public static void ResetHighScore()
    {
        HighScore = 0;
    }
}
