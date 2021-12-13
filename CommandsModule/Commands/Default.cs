namespace CommandsModule
{
    class Default : ICommand
    {
        public string FullCommand { get; private set; }

        public string CommandType { get; private set; }

        public bool IsAllowed { get; set; }

        public string Result { get; set; }
        public Default() { }
        public Default(string result) { Result = result; }
        public void ExecuteCommand()
        {
        }
    }
}
