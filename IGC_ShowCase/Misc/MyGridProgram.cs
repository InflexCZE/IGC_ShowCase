using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.ModAPI.Ingame
{
    public class MyGridProgram
    {
        public string Storage { get; set; }
        public Action<string> Echo { get; }
        public IMyProgrammableBlock Me { get; }
        public IMyGridProgramRuntimeInfo Runtime { get; }
        public IMyGridTerminalSystem GridTerminalSystem { get; }

        public IMyIntergridCommunicationSystem IGC { get; }
    }
}
