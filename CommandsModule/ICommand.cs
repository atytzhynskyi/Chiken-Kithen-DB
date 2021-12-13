namespace CommandsModule
{
    public interface ICommand
    {
        string FullCommand { get;}
        string CommandType { get;}
        bool IsAllowed { get; set; }
        string Result { get; }
        void ExecuteCommand();
    }
}
