using System;
using Autofac;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Decorate;
using DependencyInjectionWorkshop.Models;

namespace MyConsole
{
    internal class Program
    {
        private static IContainer _container;

        private static void Main(string[] args)
        {
            RegisterContainer();

            var authentication = _container.Resolve<IAuthentication>();
            var isValid = authentication.Verify("lulu", "pw", "123457");

            Console.WriteLine(isValid);
        }

        private static void RegisterContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<FakeProfile>().As<IProfile>();
            containerBuilder.RegisterType<FakeHash>().As<IHash>();
            containerBuilder.RegisterType<FakeOtp>().As<IOtp>();
            containerBuilder.RegisterType<FakeLogger>().As<ILogger>();
            containerBuilder.RegisterType<FakeSlack>().As<INotification>();
            containerBuilder.RegisterType<FakeFailedCounter>().As<IFailedCounter>();

            containerBuilder.RegisterType<AuthenticationService>().As<IAuthentication>();

            containerBuilder.RegisterType<LogDecorate>();
            containerBuilder.RegisterType<NotifyDecorator>();
            containerBuilder.RegisterType<FailedCounterDecorator>();

            containerBuilder.RegisterDecorator<NotifyDecorator, IAuthentication>();
            containerBuilder.RegisterDecorator<LogDecorate, IAuthentication>();
            containerBuilder.RegisterDecorator<FailedCounterDecorator, IAuthentication>();

            _container = containerBuilder.Build();
        }
    }

    internal class FakeLogger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"{nameof(FakeLogger)}.{nameof(Info)}({message})");
        }
    }

    internal class FakeSlack : INotification
    {
        public void PushMessage(string message)
        {
            Console.WriteLine($"{nameof(FakeSlack)}.{nameof(PushMessage)}({message})");
        }
    }

    internal class FakeFailedCounter : IFailedCounter
    {
        public void Reset(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Reset)}({accountId})");
        }

        public void Add(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Add)}({accountId})");
        }

        public int Get(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Get)}({accountId})");
            return 91;
        }

        public bool CheckAccountIsLock(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(CheckAccountIsLock)}({accountId})");
            return false;
        }

        public void Update(string accountId)
        {
        }
    }

    internal class FakeOtp : IOtp
    {
        public string GetCurrentOtp(string accountId)
        {
            Console.WriteLine($"{nameof(FakeOtp)}.{nameof(GetCurrentOtp)}({accountId})");
            return "123456";
        }
    }

    internal class FakeHash : IHash
    {
        public string GetHash(string plainText)
        {
            Console.WriteLine($"{nameof(FakeHash)}.{nameof(GetHash)}({plainText})");
            return "my hashed password";
        }
    }

    internal class FakeProfile : IProfile
    {
        public string GetPassword(string accountId)
        {
            Console.WriteLine($"{nameof(FakeProfile)}.{nameof(GetPassword)}({accountId})");
            return "my hashed password";
        }
    }
}