# PSExec
A Simple .Net wrapper to invoke psexec 


Quick Start:
```csharp
var processName = "ping.exe";
var arguments = new string[] { "-n", "4", "google.com" }

var options = new PSExec.Options()
{
  User = new PSExec.User()
  {
    Username = "Bob",
    Password = "P@55w0rd"
  }
}

using (var process = new PSExec.Process("ServerName", processName, options, arguments))
{
  process.Start();
  process.WaitForExit();
}
```
