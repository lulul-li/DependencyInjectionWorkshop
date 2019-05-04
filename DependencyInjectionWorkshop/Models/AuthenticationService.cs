using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using NLog;
using SlackAPI;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        public bool Verify(string accountId, string password, string otp)
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.com/") };
            var isLockResp = httpClient.PostAsJsonAsync("api/failedCounter/IsLock", accountId).Result;
            isLockResp.EnsureSuccessStatusCode();
            
            if (!isLockResp.Content.ReadAsAsync<bool>().Result)
            {
                throw new FailedTooManyTimesException();
            }

            string dbPassword;
            using (var connection = new SqlConnection("my connection string"))
            {
                dbPassword = connection.Query<string>("spGetUserPassword", new { Id = accountId },
                    commandType: CommandType.StoredProcedure).SingleOrDefault();
            }

            var hashPassword = new StringBuilder();
            var crypto = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
            {
                hashPassword.Append(theByte.ToString("x2"));
            }

            var response = httpClient.PostAsJsonAsync("api/otps", accountId).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"web api error, accountId:{accountId}");
            }

            var currentOtp = response.Content.ReadAsAsync<string>().Result;

            if (hashPassword.ToString() == dbPassword && currentOtp == otp)
            {
                var resetResp = httpClient.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
                resetResp.EnsureSuccessStatusCode();

                return true;
            }
            else
            {
                var addFailedCounterResp = httpClient.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
                addFailedCounterResp.EnsureSuccessStatusCode();

                var failedCounterResp = httpClient.PostAsJsonAsync("api/failedCounter/Get", accountId).Result;
                failedCounterResp.EnsureSuccessStatusCode();

                var logger = LogManager.GetCurrentClassLogger();
                logger.Info($"verify failed account : {accountId} ,failed count {failedCounterResp.Content.ReadAsAsync<int>().Result}");

                var slackClient = new SlackClient("my api token");
                slackClient.PostMessage(resp => { }, "my channel", "my message", "my bot name");

                return false;
            }
        }
    }

    public class FailedTooManyTimesException : Exception
    {
        
    }
}