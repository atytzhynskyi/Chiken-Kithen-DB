using AdvanceClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
