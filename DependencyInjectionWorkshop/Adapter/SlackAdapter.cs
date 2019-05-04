using SlackAPI;

namespace DependencyInjectionWorkshop.Adapter
{
    public class SlackAdapter
    {
        public void Notify(string myMessage)
        {
            var slackClient = new SlackClient("my api token");
            slackClient.PostMessage(resp => { }, "my channel", myMessage, "my bot name");
        }
    }
}