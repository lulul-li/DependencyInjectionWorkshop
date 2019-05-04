using SlackAPI;

namespace DependencyInjectionWorkshop.Adapter
{
    public interface INotify
    {
        void PushMessage(string myMessage);
    }

    public class SlackAdapter : INotify
    {
        public void PushMessage(string myMessage)
        {
            var slackClient = new SlackClient("my api token");
            slackClient.PostMessage(resp => { }, "my channel", myMessage, "my bot name");
        }
    }
}