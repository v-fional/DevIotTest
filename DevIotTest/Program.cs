using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System;

namespace DevIotTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string buildPath = string.Empty;

                if (args.Length == 0)
                    buildPath = @"E:\DevKitTest\Test\Build";
                else
                    buildPath = args[0];


                SerialPortListener _listener = new SerialPortListener("COM5", 115200);
                _listener.start();

                RunTestCases(buildPath);

                _listener.Stop();
            }
            catch
            {

            }
        }


        private static void RunTestCases(string buildPath)
        {
            string error = "";
            string argument = "";
            string binFile = "";


            string arduinoPath = @"C:\Program Files (x86)\Arduino\arduino_debug.exe";
            string arduinoArg = "--pref build.path={0} --verify {1}";

            string openocdPath = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Arduino15\packages\AZ3166\tools\openocd\0.10.0\bin\openocd.exe");
            string openocdArg = "-f interface/stlink-v2-1.cfg -f target/stm32f4x.cfg -c \"program {0} verify reset 0x8008000; shutdown\"";

            DirectoryInfo dir = new DirectoryInfo(@"E:\DevKitTest\Test\Arduino_Library_Test");
            FileInfo[] files = dir.GetFiles("*.ino", SearchOption.AllDirectories);

            foreach (FileInfo file in files)
            {
                Console.WriteLine("Test File >>>>> " + file.FullName);
                

                if (Directory.Exists(buildPath))
                {
                    Directory.Delete(buildPath, true);
                }
                Directory.CreateDirectory(buildPath);

                Console.WriteLine("Verify *.ino file");
                argument = string.Format(arduinoArg, buildPath, file.FullName);
                Elevated.RunProcessAsAdmin(arduinoPath, argument, out error);

                Console.WriteLine("Upload *.bin file");
                binFile = Path.Combine(buildPath, file.Name + ".bin");
                binFile = binFile.Replace('\\', '/');

                argument = string.Format(openocdArg, binFile);
                Elevated.RunProcessAsAdmin(openocdPath, argument, out error);

                Console.WriteLine("Done");
            }
            Console.WriteLine("All done, please press any key to exit....");
            Console.Read();
        }
    }
}
