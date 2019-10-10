using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Player : IPlayer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public bool IsBanned { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreationTime { get; set; }
    public Run[] Runs { get; set; }
    public SaveData SaveData { get; set; }

    public Player()
    {
        Id = Guid.NewGuid();
        CreationTime = DateTime.UtcNow;
    }

    public Player(NewPlayer newPlayer)
    {
        Id = Guid.NewGuid();
        CreationTime = DateTime.UtcNow;
        Name = newPlayer.Name;
        Password = newPlayer.Password;
        IsBanned = false;
        IsAdmin = false;
    }

}

public interface IPlayer
{
    string Name { get; }
    string Password { get; }
    bool IsAdmin { get; }
}

[Serializable]
public class NewPlayer
{
    public string Name { get; set; }
    public string Password { get; set; }
}

[Serializable]
public class Run : IRun
{
    public Guid Id { get; set; }
    public float TimeTaken { get; set; }
    public DateTime TimePosted { get; set; }
    public string Level { get; set; }

    public Run()
    {
        Id = Guid.NewGuid();
        TimePosted = DateTime.UtcNow;
    }

    public Run(NewRun newRun)
    {
        Id = Guid.NewGuid();
        TimeTaken = newRun.TimeTaken;
        TimePosted = newRun.TimePosted;
        Level = newRun.Level;
    }

    public int CompareTo(Run other)
    {
        return this.TimeTaken.CompareTo(other.TimeTaken);
    }
}

[Serializable]
public class NewRun : IRun
{
    public float TimeTaken { get; set; } //speedrun time
    public DateTime TimePosted { get; set; }
    public string Level { get; set; }
}

public interface IRun
{
    float TimeTaken { get; }
    DateTime TimePosted { get; }
    string Level { get; }
}

[Serializable]
public class SaveData
{
    public int LemmingsSavedTotal { get; set; }
    public string CurrentLevel { get; set; }
    public float PlayTime { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
}

[Serializable]
public class PlayerInformation
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsBanned { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreationTime { get; set; }
    public Run[] Runs { get; set; }
}

[Serializable]
public class PlayerArray
{
    public PlayerInformation[] InfoArray { get; set; }
}

[Serializable]
public class RunArrayHolder
{
    public Run[] RunArray { get; set; }
}
