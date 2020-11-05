﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CrocCSharpBot
{
    [ServiceContract]
    public interface IContolService
    {
        [OperationContract]
        string Query();

        [OperationContract]
        void StartTrace(string ip);

        [OperationContract]
        void StopTrace();
    }
}
