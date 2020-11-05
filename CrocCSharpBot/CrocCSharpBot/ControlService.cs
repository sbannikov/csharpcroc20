using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CrocCSharpBot
{
    /// <summary>
    /// Управляющий сервис
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Single)]
    public class ControlService : IContolService
    {
        public string Query()
        {
            return "Привет!";
        }
    }
}
