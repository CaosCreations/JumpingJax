using UnityEngine;
[CreateAssetMenu(fileName = "SkipCheckpointCommand", menuName = "Developer Console/Commands/SkipCheckpointCommand")]
public class CommandSkipCheckpoint : ConsoleCommand
{
    // TODO: find a new way to do this now that checkpoints aren't ordered
    public override void Process(string[] args)
    {
        //int parsedLevel;
        //if (int.TryParse(args[0], out parsedLevel))
        //{
        //    PlayerProgress player = FindObjectOfType<PlayerProgress>();
        //    Checkpoint[] components = GameObject.FindObjectsOfType<Checkpoint>();
        //    foreach (Checkpoint checkpoint in components)
        //    {
        //        if (checkpoint.level == parsedLevel)
        //        {
        //            player.currentCheckpoint = checkpoint;
        //            player.Respawn();
        //        }
        //    }
        //}
        //else
        //{
        //    Debug.LogError("Could not process checkpoint number. Please use and integer");
        //}
    }
}
