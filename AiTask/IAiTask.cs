using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaClosedAi.AiTask
{
    public interface IAiTask
    {
        bool IsRunning { get; }

        bool Run();
        bool Stop();
    }
}
