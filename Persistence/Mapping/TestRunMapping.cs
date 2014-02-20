﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain.Models;

namespace TestLab.Infrastructure.Persistence.Mapping
{
    class TestRunMapping : EntityTypeConfiguration<TestRun>
    {
        public TestRunMapping()
        {
            ToTable("TestRuns");

            HasKey(z => z.Id);

            HasRequired(z => z.TestBuild)
                .WithMany()
                .HasForeignKey(z => z.TestBuildId);

            HasRequired(z => z.TestPlan)
                .WithMany(f => f.TestRuns)
                .HasForeignKey(z => z.TestPlanId);
        }
    }
}
