using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Core.Interfaces
{
    public interface IQueueAccessor
    {
        IEnumerable<string> ListQueueUrls();
        void HandleAndDeleteMessage(Action<string> messageHandler);
    }
}
