using static UnityEngine.PlayerPrefs;

public static class SavedData
{
    private const string _highScoreKey = "highscore";

    public static int HighScore
    {
        get => GetInt(_highScoreKey);
        set
        {
            SetInt(_highScoreKey, value);
            Save();
        }
    }
}
