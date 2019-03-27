using System.ComponentModel;

namespace PSExec
{
    public class Options
    {
        /// <summary>
        /// User credentials for PsExec process
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// [ -accepteula ]
        /// Suppress the display of the license dialog.
        /// </summary>
        public bool AcceptEULA { get; set; } = true;

        /// <summary>
        /// [ -c ]
        /// Copy the program (command) to the remote system for execution.
        /// If you omit the -c option then the application must be in the system path on the remote system.
        /// </summary>
        public CopyFlag? Copy { get; set; }

        /// <summary>
        /// [ -d ]
        /// Don’t wait for the application to terminate.
        /// Only use for non-interactive applications.
        /// </summary>
        public bool Wait { get; set; }

        /// <summary>
        /// [ -e ]
        /// Do NOT load the specified account’s profile.
        /// (In early versions of PSEXEC: Load the user account's profile, don’t use with -s)
        /// </summary>
        public bool DontLoadAccount { get; set; }

        /// <summary>
        /// [ -h ]
        /// Run with the account's elevated token, if available. (Vista or higher)
        /// </summary>
        public bool RunWithAccount { get; set; }

        /// <summary>
        /// [ -i ]
        /// Interactive - Run the program so that it interacts with the desktop on the remote system.
        /// If no session is specified, the process runs in the console session.
        /// </summary>
        public bool Interactive { get; set; }

        /// <summary>
        /// [ -l ]
        /// Limited - Run process as limited user.  Run with Low Integrity.
        /// Strips the Administrators group and allows only privileges assigned to the Users group.
        /// </summary>
        public bool Limited { get; set; }

        /// <summary>
        /// [ -n s ]
        /// Specify a timeout (s seconds) for connecting to the remote computer.
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// [ -w directory ]
        /// Set the working directory of the process (relative to the remote computer).
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// [ -x ]
        /// Display the UI on the Winlogon desktop (local system only).      
        /// </summary>
        public bool DisplayUI { get; set; }

        /// <summary>
        /// These options will run the process at a different priority.
        /// also -background (Vista and above) will run at low memory and I/O priority.
        /// </summary>
        public CpuPriority? Priority { get; set; }

        /// <summary>
        /// -c      Copy the program (command) to the remote system for execution.
        /// -c -f   Copy even if the file already exists on the remote system.
        /// -c -v   Copy only if the file is a higher version or is newer than the remote copy.
        /// </summary>
        public enum CopyFlag
        {
            [Description("Copy the program (command) to the remote system for execution.")]
            CopyRemote,

            [Description("Copy even if the file already exists on the remote system")]
            CopyRemoteIfExits,

            [Description("Copy the program (command) to the remote system for execution.")]
            CopyRemoteIfNewer,

        }

        /// <summary>
        /// These options will run the process at a different priority.
        /// also -background (Vista and above) will run at low memory and I/O priority.
        /// </summary>
        public enum CpuPriority
        {
            Low,
            BelowNormal,
            AboveNormal,
            High,
            Realtime,
            Background
        }


        public static string GetOptionsString(Options options)
        {
            var arguments = string.Empty;

            if (options.User != null && options.User.Username != null && options.User.Password != null)
            {
                arguments += $" -u {options.User.Username} -p {options.User.Password}";
            }

            arguments += (options.RunWithAccount) ? " -h" : string.Empty;
            arguments += (options.DontLoadAccount) ? " -e" : string.Empty;
            arguments += (options.Interactive) ? " -i" : string.Empty;
            arguments += (options.Limited) ? " -l" : string.Empty;
            arguments += (options.DisplayUI) ? " -x" : string.Empty;

            arguments += (options.AcceptEULA) ? " -accepteula" : string.Empty;

            arguments += (options.Timeout != null) ? $" -n {options.Timeout}" : string.Empty;
            arguments += (!string.IsNullOrEmpty(options.WorkingDirectory)) ? $" -w {options.WorkingDirectory}" : string.Empty;

            if (options.Priority != null)
            {
                switch (options.Priority)
                {
                    case CpuPriority.Low:
                        arguments += " -low";
                        break;
                    case CpuPriority.BelowNormal:
                        arguments += " -belownormal";
                        break;
                    case CpuPriority.AboveNormal:
                        arguments += " -abovenormal";
                        break;
                    case CpuPriority.High:
                        arguments += " -high";
                        break;
                    case CpuPriority.Realtime:
                        arguments += " -realtime";
                        break;
                    case CpuPriority.Background:
                        arguments += " -background";
                        break;
                    default:
                        break;
                }
            }

            if (options.Copy != null)
            {
                switch (options.Copy)
                {
                    case CopyFlag.CopyRemote:
                        arguments += " -c ";
                        break;
                    case CopyFlag.CopyRemoteIfExits:
                        arguments += " -f -c ";
                        break;
                    case CopyFlag.CopyRemoteIfNewer:
                        arguments += " -v -c ";
                        break;
                    default:
                        break;
                }
            }

        
            return arguments + " ";
        }
    }

    public class User
    {
        /// <summary>
        /// [ -u ]
        /// Specify a user name for login to remote computer(optional).
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// [ -p ]
        /// Specify a password for user (optional). Passed as clear text.
        /// If omitted, you will be prompted to enter a hidden password.
        /// </summary>
        public string Password { get; set; }
    }
}
