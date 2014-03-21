User Guide
==========
Basically go follow steps to engage the testing.

1. Create Test Project
2. Build Test Project
3. Create Test Plan
4. Create Test Session With Test Agent(s)
5. View Test Runs and Wait for Test Results

Test Project
------------
A test project defined as the aggregate root which has test cases, test plans, and test sessions.

The project's source code would be pulled to local from a remote repository during the build process.

Meanwhile, test cases would be published.

### Create Project
There is a button "Create New" for creating a new Test Project.

> - Project Name must be unique, since it would be used as local work dir
>
> - Currently, only support git repository
>
> - Currently, only Cucumber Test Driver
>
> - Build script and output are optional, apparently empty for cucumber, since no special build process

### Build Project
There is a green button "Start New Build" to build the project.

A Build Job would run as Background task, so you may leave it and come back later once it started.

The process flow of a Build:

    pull source -> build project -> archive build -> publish test cases

Test Plan
---------
A test plan defined as a collection of test cases.

Open a project, then go plans page via top menu bar.

### Create Plan
There is a button "Create New" for creating new test plan.

Pick the test cases from dual list box, and of course give a sensible name for the plan.

    e.g. "Smoke Test Plan"

> Test Cases are order by Name ascending

Test Session
------------
A test session defined as the session for running test cases of a test plan with environment configurations.

Open a project, then go sessions page via top menu bar to view all test sessions of the project.

Each row has a progress bar: green means passed, red means failed, blue means running.

### Create Test Session
There is a button "Create New" on sessions page for creating new test session.

> - A test plan must be specified for this test session.
>
> - The project must has a build.
>
> - Only Published Test cases in the Test Plan would be picked to create Test Run in Test Session

As Convention, the name for test session should be the test plan name without "plan" suffix and with a short time stamp.

    e.g. "Smoke Test Plan" -> "Smoke Test 0314"

Test Agent must be specified for this test session, multiple Test Agents are supported via commas.

    e.g. "server1, server2, server3"

You may speed up the Test Session by multiple Test Agents.

### Test Agent
Test Agent is the user account and machine used for running test case.

TestLab would create schedule task run as the user account on that machine.

You may use follow cmd to query the jobs under root folder on that machine.

    schtasks /query /S [server]/

> - The machine must be able to run the test case with the user account, you may try logon that machine with the user to verify it
>
> - The user must be able to logon the machine when starting test case, otherwise the schedule task cannot start
>
> - Do not run the test case manually with the user logon that machine at same time when schedule task running

### Start Test Session
Open a Test Session, there is a green button "Start" for starting the test session.

The Job would run as background task after test session started, so you may leave it there and come back later to see if all completed.

### Test Runs and Results
Test Run is the task for running test case, the Test Result would be saved in the Test Run after completed.

Open a Test Session, there is a list of Test Runs with Results displayed in a grid.

Rows in green means passed, red means failed.

Each Result's Summary could be expanded to view details, including the output file path.