public interface IConsoleCommand
{
    string Command { get; }
    string Description { get; }

    void Process(string[] args);
}