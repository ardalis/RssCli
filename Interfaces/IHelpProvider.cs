namespace RssCli.Interfaces
{
    public interface IHelpProvider
    {
        void ShowMainHelp();
        void ShowCommandHelp(string commandName);
    }
}
