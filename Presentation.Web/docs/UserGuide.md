User Guide
==========
Basically go follow steps to engage the testing.

1. Create Test Project
2. Build Test Project
3. Create Test Plan
4. Create Test Session for Test Plan With Test Agent(s)
5. View Test Runs and Wait for Test Results

Test Agent
----------
Test Agent is the machine for running test case or build job.

On the machine, we need install a windows service "TestLabAgentService" with a user account.

It will keep talking to TestLab, and get build jobs or test cases to run on this machine.

> - TestLab.AgentService.exe could also run in console, while it would show all the windows (like browser) during running a testcase.

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

Test Build
----------
A test build defined as a build of project.

Open a project, go Builds page by click "Builds" in sidebar to view all test builds of the project.

### Build Project
There is a button "Create New" for creating new build for this project.

A test agent is required for running the build job.

A Build Job would be started on that Agent in 1 min once created.

The process flow of a Build:

    pull source -> build project -> archive build -> publish test cases

> - If the Agent list empty, that means no Agents online at this time.

Test Plan
---------
A test plan defined as a collection of test cases.

Open a project, then go Plans page via side bar to view all test plans of the project.

### Create Plan
There is a button "Create New" for creating new test plan.

Pick the test cases from dual list box, and give a sensible name without "test plan" suffix for the plan.

    e.g. "Smoke", "Regression", "Daily"

> Test Cases are order by Name ascending

Test Session
------------
A test session defined as the session for running test cases of a test plan with environment configurations.

Open a project, then go sessions page via side bar to view all test sessions of the project.

Each row has a progress bar: green means passed, red means failed, blue means running.

### Create Test Session
There is a button "Create New" on sessions page for creating new test session.

Multiple Agents could be picked for better concurrency.

Test Session would be started as Test Queue job(s) on agent(s) in 1 min once created.

> - If the Agent list empty, that means no Agents online at this time.
>
> - A test plan must be specified for this test session.
>
> - The project must has a completed build.
>
> - Only Published Test cases in the Test Plan would be runned in Test Session

### Test Config
Test Config is the variables for running a test case.

It might be set as Environment Variables or something else according to the Test Driver implements.

For cucumber test cases, we can set a json format string contains the key values like browser or website.

	e.g. { "browser": "firefox", "site": "https://expediasb.service-now.com/navpage.do" }
	
Then in ruby code, we can get this by key "testconfig.json" from Environment Variables.

### Restart Failed cases in Test Session
Only available when some cases failed in this test session.

Open a Test Session, there is a green button "Restart Failed Cases".

Those Failed Cases would restarted on agents in 1 min once button clicked.

### Edit Test Session
Sometimes, we want restarting the whole test session with new build or updated test plan.

Test Session would be refreshed as new created one, and started on agents in 1 min.

### Test Run and Test Result
Test Run is the task for running test case, the Test Result would be saved in the Test Run after completed.

One Test Run for One Test Case in a Test Session.

Open a Test Session, there is a list of Test Runs with Results displayed in a grid.

Rows in green means passed, red means failed.

Each Result could be expanded to view details, including the output file path.