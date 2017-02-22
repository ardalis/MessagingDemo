using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Core.Interfaces
{
    public interface ISendNotifications
    {
        void SendNotification(string topic, string message, string subject);
        string GetTopicARN(string id);
        void ListTopics();
    }
}
