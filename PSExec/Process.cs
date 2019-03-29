using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PSExec
{
    /// <summary>
    /// 
    /// </summary>
    public class Process : IDisposable
    {
        public bool Use64Bit { get; set; } = true;    
        private System.Diagnostics.Process PsExecProcess { get; set; }

        #region Constructors

        public Process()
        {
        }

        public Process(string ComputerName, string ProgramName)
        {
            BuildProcess(new string[] { ComputerName }, ProgramName, new Options(), new string[] { });
        }

        public Process(string ComputerName, string ProgramName, Options options)
        {
            BuildProcess(new string[] { ComputerName }, ProgramName, options, new string[] { });
        }

        public Process(string ComputerName, string ProgramName, Options options, string[] Arguments)
        {
            BuildProcess(new string[] { ComputerName }, ProgramName, options, Arguments);
        }

        public Process(string[] ComputerNames, string ProgramName, Options options, string[] Arguments)
        {
            BuildProcess(ComputerNames, ProgramName, options, Arguments);
        }

        #endregion

        private void BuildProcess(string[] ComputerNames, string ProgramName, Options options, string[] Arguments)
        {
            var optionsString = Options.GetOptionsString(options);

            var argumentsString = string.Join(@" ", Arguments.Select(a => $@"""{a}"""));

            var computers = string.Join(",", ComputerNames.Select(name =>
            {
                if (!name.StartsWith(@"\\")) return @"\\" + name;
                else return name;
            }));

            PsExecProcess = new System.Diagnostics.Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = (Use64Bit) ? @".\PsExec64.exe" : @".\PsExec.exe", 
                    Arguments = computers + " " + optionsString + " " + ProgramName + " " + argumentsString,
                }
            };

            PsExecProcess.OutputDataReceived += PsExecProcess_OutputDataReceived;
            PsExecProcess.ErrorDataReceived += PsExecProcess_ErrorDataReceived;

            PsExecProcess.Exited += PsExecProcess_Exited;
            PsExecProcess.Disposed += PsExecProcess_Disposed;
        }

        public void Start()
        {
            if (PsExecProcess is null)
                throw new Exception("Process is NULL.");

            PsExecProcess.Start();
            PsExecProcess.BeginErrorReadLine();
            PsExecProcess.BeginOutputReadLine();            
        }

        public void WaitForExit()
        {
            if (PsExecProcess is null)
                throw new Exception("Process is NULL.");

            PsExecProcess.WaitForExit();
        }

        public bool WaitForExit(int milliseconds)
        {
            if (PsExecProcess is null) throw new Exception("Process is null.");
            return PsExecProcess.WaitForExit(milliseconds);
        }

        #region Process Events

        private void PsExecProcess_Disposed(object sender, EventArgs e)
        {
            // TODO
        }

        private void PsExecProcess_Exited(object sender, EventArgs e)
        {
            // TODO
        }

        private async void PsExecProcess_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            await Console.Error.WriteLineAsync(e.Data);
        }

        private async void PsExecProcess_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            await Console.Out.WriteLineAsync(e.Data);
        }


        #endregion

        #region IDispose
        private bool disposed { get; set; } = false;

        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();

                if (PsExecProcess != null)
                {
                    try
                    {
                        if (!PsExecProcess.HasExited)
                            PsExecProcess.Kill();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    try
                    {
                        PsExecProcess.Dispose();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            disposed = true;
        }

        ~Process()
        {
            Dispose(false);
        }

        #endregion
    }
}
