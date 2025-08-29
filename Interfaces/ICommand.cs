namespace RssCli.Interfaces
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        Task<int> ExecuteAsync(string[] args);
        void ShowHelp();
    }
}
