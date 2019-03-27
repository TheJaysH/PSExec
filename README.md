# PSExec
A Simple .Net wrapper to invoke psexec 

Quick Start:
```csharp
// Declare server & command details
string serverHostname = "Server-01";
string processName = "ping.exe";
string[] arguments = new string[] { "-n", "4", "google.com" };

// Setup the options
PSExec.Options options = new PSExec.Options()
{
  User = new PSExec.User()
  {
    Username = "Bob",
    Password = "P@55w0rd"
  }
};

// Run the process
using (PSExec.Process process = new PSExec.Process(serverHostname, processName, options, arguments))
{
  process.Start();
  process.WaitForExit();
}
```
