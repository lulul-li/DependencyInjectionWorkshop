using System;
using System.Net.Http;

namespace DependencyInjectionWorkshop.Models
{
    public class FailedCounter
    {
        public void ResetFailedCounter(string accountId)
        {
            var resetResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            resetResp.EnsureSuccessStatusCode();
        }

        public void AddFailedCounter(string accountId)
        {
            var addFailedCounterResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCounterResp.EnsureSuccessStatusCode();
        }

        public int GetFailedCount(string accountId)
        {
            var failedCounterResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Get", accountId).Result;
            failedCounterResp.EnsureSuccessStatusCode();
            var failedCount = failedCounterResp.Content.ReadAsAsync<int>().Result;
            return failedCount;
        }

        public void CheckAccountIsLock(string accountId)
        {
            var isLockResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/IsLock", accountId).Result;
            isLockResp.EnsureSuccessStatusCode();

            if (!isLockResp.Content.ReadAsAsync<bool>().Result)
            {
                throw new FailedTooManyTimesException();
            }
        }
    }
}