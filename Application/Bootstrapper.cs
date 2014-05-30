﻿using Ninject;
using NPatterns;
using NPatterns.Messaging;
using TestLab.Domain;

namespace TestLab.Application
{
    public class Bootstrapper
    {
        public static void Initialize(IKernel kernel)
        {
            kernel.Load("TestLab.Infrastructure.*.dll");

            kernel.Bind<IHandler<TestBuild>>().To<TestBuildJobHandler>();
            kernel.Bind<IHandler<TestQueue>>().To<TestQueueJobHandler>();
            kernel.Bind<IHandler<TestRun>>().To<TestRunJobHandler>();

            kernel.Bind<IMessageBus>()
                  .ToConstructor(ctorArg => new MessageBus(type => ctorArg.Context.Kernel.GetAll(type)))
                  .InSingletonScope();
        }        
    }
}