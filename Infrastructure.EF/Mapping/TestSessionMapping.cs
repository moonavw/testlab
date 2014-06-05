﻿using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestSessionMapping : EntityTypeConfiguration<TestSession>
    {
        public TestSessionMapping()
        {
            HasKey(z => z.Id);

            HasRequired(z => z.Project)
                .WithMany(f => f.Sessions)
                .Map(m => m.MapKey("TestProjectId"))
                .WillCascadeOnDelete(false);

            HasRequired(z => z.Build)
                .WithMany()
                .Map(m => m.MapKey("TestBuildId"))
                .WillCascadeOnDelete(false);

            HasRequired(z => z.Plan)
                .WithMany(f => f.Sessions)
                .Map(m => m.MapKey("TestPlanId"));
        }
    }
}