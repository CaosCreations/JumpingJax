using UnityEngine;
[CreateAssetMenu(fileName = "New SkipCheckpoint Command", menuName = "Developer Console/Commands/SkipCheckpoint Command")]
public class CommandSkipCheckpoint : ConsoleCommand
{
    public override void Process(string[] args)
    {
        int parsedLevel;
        if (int.TryParse(args[0], out parsedLevel))
        {
            PlayerProgress player = FindObjectOfType<PlayerProgress>();
            Checkpoint[] components = GameObject.FindObjectsOfType<Checkpoint>();
            foreach (Checkpoint checkpoint in components)
            {
                if (checkpoint.level == parsedLevel)
                {
                    player.currentCheckpoint = checkpoint;
                    player.Respawn();
                }
            }
        }
        else
        {
            Debug.LogError("Could not process checkpoint number. Please use and integer");
        }
    }
}
