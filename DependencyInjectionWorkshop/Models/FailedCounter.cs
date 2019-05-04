using System;
using System.Net.Http;
using DependencyInjectionWorkshop.Exception;

namespace DependencyInjectionWorkshop.Models
{
    public interface IFailedCounter
    {
        void Reset(string accountId);
        void Add(string accountId);
        int Get(string accountId);
        bool CheckAccountIsLock(string accountId);
    }

    public class FailedCounter : IFailedCounter
    {
        public void Reset(string accountId)
        {
            var resetResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            resetResp.EnsureSuccessStatusCode();
        }

        public void Add(string accountId)
        {
            var addFailedCounterResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCounterResp.EnsureSuccessStatusCode();
        }

        public int Get(string accountId)
        {
            var failedCounterResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Get", accountId).Result;
            failedCounterResp.EnsureSuccessStatusCode();
            var failedCount = failedCounterResp.Content.ReadAsAsync<int>().Result;
            return failedCount;
        }

        public bool CheckAccountIsLock(string accountId)
        {
            var isLockResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/IsLock", accountId).Result;
            isLockResp.EnsureSuccessStatusCode();

            return isLockResp.Content.ReadAsAsync<bool>().Result;
            
        }
    }
}