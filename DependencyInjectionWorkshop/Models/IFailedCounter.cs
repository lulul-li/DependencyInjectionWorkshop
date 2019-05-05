namespace DependencyInjectionWorkshop.Models
{
    public interface IFailedCounter
    {
        void Reset(string accountId);

        void Add(string accountId);

        int Get(string accountId);

        bool CheckAccountIsLock(string accountId);

        void Update(string accountId);
    }
}