using System;

namespace TestLab.Infrastructure
{
    public interface IStartable
    {
        DateTime? Started { get; set; }

        DateTime? Completed { get; set; } 
    }
}