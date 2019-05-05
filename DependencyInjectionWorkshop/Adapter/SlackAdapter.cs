using SlackAPI;

namespace DependencyInjectionWorkshop.Adapter
{
    public class SlackAdapter : INotification
    {
        public void PushMessage(string myMessage)
        {
            var slackClient = new SlackClient("my api token");
            slackClient.PostMessage(resp => { }, "my channel", myMessage, "my bot name");
        }
    }
}