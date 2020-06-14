using MediatR;
using Microsoft.Azure.ServiceBus;
using ServiceBusCLI.Features.GenerateSecurityAccessSignature;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using ServiceBusCLI.Utils;
using ServiceBusCLI.Features.SendMessage;
using Common;

namespace ServiceBusCLI.Features.SendJob
{

    public static class SendJob 
    {
 
        public class SendJobHandler :  SendMessage<Job>.Handler { }


    }
}
