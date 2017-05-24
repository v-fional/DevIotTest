using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;

namespace DevIotTest
{
    public static class Elevated
    {
        #region Common

        /// <summary>
        ///     This function will elevate to execute the command as admin. It can be used for actions to change
        ///     registry, folder, and files that require admin privilege.
        /// 
        ///     If you need to execute your test with admin privilege, please use "Run As Admin" flag in MadDog.
        /// </summary>
        /// <param name="exeFileName">Executable to run in elevated process</param>
        /// <param name="exeArgument">Parameters for the executable</param>
        /// <param name="errorString">Output string for error messages</param>
        /// <returns>
        ///     true - everything passed
        ///     false - if exception occurs or ExitCode != 0
        /// </returns>
        public static bool RunProcessAsAdmin(string exeFileName, string exeArgument, out string errorString)
        {
            bool bPass = true;

            errorString = "";
            Process proc = null;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                //psi.Verb = "runas";
                psi.FileName = exeFileName;
                psi.Arguments = exeArgument;

                proc = Process.Start(psi);
                int timeout = 2;

                while (timeout >= 0 && !proc.HasExited)
                {
                    proc.WaitForExit(60000);
                    timeout--;
                }

                // sleep for 1 second to ensure the action is complete 
                System.Threading.Thread.Sleep(1000);
                if (proc.ExitCode != 0)
                {
                    bPass = false;
                    errorString += "Process exited with code [ " + proc.ExitCode + " ].";   // TODO: include the standard error/output?
                }
            }
            catch (Exception ex)
            {
                bPass = false;
                errorString += string.Format("Failed to execute the command as admin: [ {0} {1} ]{2}", exeFileName, exeArgument, Environment.NewLine);
                errorString += ex.StackTrace;
            }
            finally
            {
                if (!proc.HasExited)
                {
                    proc.Kill();
                }
            }

            return bPass;
        }

        #endregion Common

        #region Process
        /// <summary>
        /// kill a process with the admin privilege
        /// </summary>
        /// <param name="p"></param>
        public static void KillProcessAsAdmin(string processName)
        {
            string error = "";

            try
            {
                string exeName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe");   // %windir%\System32\cmd.exe
                string argument = string.Format("/C taskkill /f /im {0}", processName);

                RunProcessAsAdmin(exeName, argument, out error);

                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to kill the process", ex);
            }
        }
        #endregion
    }
}
