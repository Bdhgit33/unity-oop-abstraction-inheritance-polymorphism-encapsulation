using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HighScoreEntry
{
    public string playerName;
    public float timeSeconds;

    public HighScoreEntry(string playerName, float timeSeconds)
    {
        this.playerName = playerName;
        this.timeSeconds = timeSeconds;
    }
}

[System.Serializable]
public class HighScoreData
{
    public List<HighScoreEntry> entries = new List<HighScoreEntry>();
}
