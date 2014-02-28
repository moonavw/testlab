﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class TestService : ITestService
    {
        private readonly ITestBuilder _builder;
        private readonly IArchiver _archiver;
        private readonly IEnumerable<ITestDriver> _drivers;
        private readonly IEnumerable<IRepoPuller> _pullers;

        public TestService(
            IEnumerable<IRepoPuller> pullers,
            ITestBuilder builder,
            IArchiver archiver,
            IEnumerable<ITestDriver> drivers)
        {
            _pullers = pullers;
            _builder = builder;
            _archiver = archiver;
            _drivers = drivers;
        }

        public async Task Build(TestProject project)
        {
            var puller = _pullers.FirstOrDefault(z => z.CanPull(project.RepoPathOrUrl));
            if (puller == null) throw new NotSupportedException("no puller for this project");
            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this project");

            //pull
            await puller.Pull(project.RepoPathOrUrl, project.WorkDir);

            //build
            var build = project.Build = await _builder.Build(project);

            //archive
            await _archiver.Archive(project.BuildOutputDir, build.ArchivePath);
            build.Archived = DateTime.Now;

            //publish
            var tests = (await driver.Publish(project)).ToList();
            var toDel = project.Cases.ExceptBy(tests, z => z.FullName).ToList();
            var toAdd = tests.ExceptBy(project.Cases, z => z.FullName).ToList();

            toDel.ForEach(z =>
            {
                z.Plans.Clear();
                z.Published = null;
                //project.Cases.Remove(z);
            });
            toAdd.ForEach(z =>
            {
                project.Cases.Add(z);
            });
        }

        private async Task OnSessionStarting(TestSession session)
        {
            var project = session.Plan.Project;

            //find build
            var build = session.Build;
            if (build.Completed == null)
            {//pick project's build if session's null
                build = session.Build = project.Build;
            }
            if (build.Completed == null) throw new InvalidOperationException("no build for this test session");

            //extract
            await _archiver.Extract(build.ArchivePath, Path.Combine(session.RemoteBuildRoot, build.Name));
        }

        public async Task Run(TestSession session)
        {
            var project = session.Plan.Project;
            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this test session");

            //start
            session.Started = DateTime.Now;

            await OnSessionStarting(session);

            //runs
            var runs = session.Runs;
            if (runs.Count == 0) throw new InvalidOperationException("no tests for this test session");

            foreach (var t in runs)
            {
                t.Result = await driver.Run(t);
            }
            session.Completed = DateTime.Now;
        }

        public async Task Run(TestRun run)
        {
            var session = run.Session;
            var project = session.Plan.Project;
            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this test session");

            if (session.Started == null)
            {
                session.Started = DateTime.Now;
                await OnSessionStarting(session);
            }

            //run
            run.Result = await driver.Run(run);
        }
    }
}