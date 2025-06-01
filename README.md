This is a Unity Course Project covering the final course cariculum for demonstrating the use of OOP. (Object Oriented Programming)

MATCHiT 
Is a card matching game. You must match 8 pairs of cards in the fastest time possible. The fastest time will be at the top of the high scores table.!

I choose this project to demonstration the 4 pricipals of object oeriented programming for its simplicity. so that I can focus on making good code whilst at the same time using the appropriate scenarios to adhere to the 4 following pricipals: 

- Abstraction: 
- Polymophism
- Inheritence
- Encapsulation

Below is 4 code snippets form my project that show the use of these pricipals in Action: 


----------------------------------------------------------------------
ENCAPSULATION: 

public class HighScoreController : MonoBehaviour
{
   
    private List<int> _scores = new List<int>();

    // Public method controls how new scores are added:
    public void AddScore(int score)
    {
        _scores.Add(score);
        _scores.Sort();
        if (_scores.Count > 10)
            _scores.RemoveAt(_scores.Count - 1);
    }

    
    public IReadOnlyList<int> Scores => _scores.AsReadOnly();
}
-------------------------------------------------------------------------
Explaination: Here we Encapulate the score data into read only lists so that the data can not be accidentally overwritten by outside changes. 
Here we only let other people save and view the data in a safe way. 

------------------------------------------------------------------
2. Polymorphism

-------------------------------------------------------------------

public abstract class CardController : MonoBehaviour
{
    public abstract void OnReveal();
}

public class TextCard : CardController
{
    public override void OnReveal()
    {
        // Show text
        Debug.Log("Revealing text card: " + gameObject.name);
    }
}

public class ImageCard : CardController
{
    public override void OnReveal()
    {
        // Show image
        Debug.Log("Revealing image card with sprite");
    }
}
-------------------------------------------------------------------------
Explaination:  You call someCard.OnReveal(), and at runtime Unity decides—“Is it a TextCard or an ImageCard?”—and runs the right code using Method overiding and overloading techniques. 
---------------------------------------------------------------------------------------------------------------------------------------------------------------
Method Overloading Example - Polymorphism - Continued

Here I use use the same method name but with different parameters.

public class ScoreRow : MonoBehaviour
{
    // Version 1: set name + time
    public void SetData(string name, int time)
    {
        // fill two fields
    }

    // Version 2: set name + time + matches
    public void SetData(string name, int time, int matches)
    {
        // fill three fields
    }
}
When you call SetData("Alice", 42), it uses the first version; if you call SetData("Bob", 58, 10), it uses the second version. 
-------------------------------------------------------------------------------------------------------------------------------------------------

 Inheritance

 One class (child - TitleScreenController, CardMAtchController etc.) reuses code from another class (parent - GameController) and adds its own bits.  


--------------------------------------------------------------------------------------------------------------------------
public class GameController : MonoBehaviour
{
    public virtual void OnQuit()
    {
        Debug.Log("Quitting game.");
    }

    public abstract void OnStart();
}

public class TitleScreenController : GameController
{
    // Inherits OnQuit() as-is
    public override void OnStart()
    {
        Debug.Log("Title screen: start button pressed.");
    }
}

public class CardMatchController : GameController
{
    public override void OnStart()
    {
        Debug.Log("Card match: load or restart level.");
    }

    public override void OnQuit()
    {
        Debug.Log("Clearing saved data, then quitting.");
        base.OnQuit();
    }
}
--------------------------------------------------------------------------------------------------------------------------------------
+ Both TitleScreenController and CardMatchController share the “quit” logic from GameController but each defines its own start up behavior.

--------------------------------------------------------------------------------------------------------------------------------------
4. Abstraction
- Here we hide code details behind a simple, high-level interface (SaveLoadManager.savePlayerData etc) so callers don’t worry about the nitty-gritty.
-------------------------------------------------------------------------------------------------------------------------

public static class SaveLoadManager
{
    // Call this, and we handle JSON + file paths internally:
    public static void SavePlayerData(string name, int bestTime)
    {
        var data = new PlayerData { Name = name, BestTime = bestTime };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "save.json"), json);
    }

    public static PlayerData LoadPlayerData()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (!File.Exists(path)) return new PlayerData();
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<PlayerData>(json);
    }
}

[Serializable]
public class PlayerData
{
    public string Name;
    public int BestTime;
}
------------------------------------------------------------------------------------------------------------------------------------------
Other scripts just call SaveLoadManager.SavePlayerData(...) or LoadPlayerData() and never need to know about JSON or file paths.


This concludes that all the object oriented principals are in full action making MATCHiT card matching game for the final project for Junio Programmer Unity Course. 

Please please the game on WebGL and enjoy!


