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
            CheckAccountIsLock(accountId);

            var dbPassword = GetDbPassword(accountId);

            var hashPassword = GetHashPassword(password);

            var currentOtp = GetCurrentOtp(accountId);

            if (hashPassword.ToString() == dbPassword && currentOtp == otp)
            {
                ResetFailedCounter(accountId);

                return true;
            }
            else
            {
                AddFailedCounter(accountId);

                var failedCount = GetFailedCount(accountId);

                log($"verify failed account : {accountId} ,failed count {failedCount}");

                Notify("my message");

                return false;
            }
        }

        private static int GetFailedCount(string accountId)
        {
            var failedCounterResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Get", accountId).Result;
            failedCounterResp.EnsureSuccessStatusCode();
            return failedCounterResp.Content.ReadAsAsync<int>().Result;
        }

        private static void Notify(string myMessage)
        {
            var slackClient = new SlackClient("my api token");
            slackClient.PostMessage(resp => { }, "my channel", myMessage, "my bot name");
        }

        private static void log(string message)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info(
                message);
        }

        private static void AddFailedCounter(string accountId)
        {
            var addFailedCounterResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCounterResp.EnsureSuccessStatusCode();
        }

        private static void ResetFailedCounter(string accountId)
        {
            var resetResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            resetResp.EnsureSuccessStatusCode();
        }

        private static string GetCurrentOtp(string accountId)
        {
            var response = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/otps", accountId).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"web api error, accountId:{accountId}");
            }

            return response.Content.ReadAsAsync<string>().Result;
        }

        private static StringBuilder GetHashPassword(string password)
        {
            var hashPassword = new StringBuilder();
            var crypto = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
            {
                hashPassword.Append(theByte.ToString("x2"));
            }

            return hashPassword;
        }

        private static string GetDbPassword(string accountId)
        {
            string dbPassword;
            using (var connection = new SqlConnection("my connection string"))
            {
                dbPassword = connection.Query<string>("spGetUserPassword", new {Id = accountId},
                    commandType: CommandType.StoredProcedure).SingleOrDefault();
            }

            return dbPassword;
        }

        private static void CheckAccountIsLock(string accountId)
        {
            var isLockResp = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/IsLock", accountId).Result;
            isLockResp.EnsureSuccessStatusCode();

            if (!isLockResp.Content.ReadAsAsync<bool>().Result)
            {
                throw new FailedTooManyTimesException();
            }
        }
    }

    public class FailedTooManyTimesException : Exception
    {

    }
}