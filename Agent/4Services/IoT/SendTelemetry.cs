using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Agent._4Services;
using ConsoleApplication1;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Agent;
using Microsoft.ServiceBus.Messaging;
using RAT.ZTry;

namespace MyDeviceTest
{
    class DeviceTest
    {
        static DeviceClient deviceClient;
        private BackgroundWorker worker;
        private bool kill = false;
        private static string iotHubUri = "ManageIoT2.azure-devices.net";
        static string deviceKey = "z0InlHLvCuQ7w6H5d0eaC+puTcB9+/Dsi1W/HdlSY4k=";
        private string device = "Device_1";

        PerformanceCounters information;
        private TelemetryDatapoint telemetry;
        private static CommandDatapoint command;
        private static string newString;
        //Sending messages to the IoT hub

        private async Task SendDeviceToCloudMessagesAsync()
        {
            information = new PerformanceCounters();
            //Loop for sending messages
            while (!kill)
            {
                //Creating a telemetry object, setting data, sending over Iot Azure
                telemetry = new TelemetryDatapoint(device);
                //Cpu
                telemetry.Cpu = information.GetCPU();
                telemetry.Cpu2 = information.GetFrequency();
                telemetry.Percent = information.GetPercent();
                telemetry.Thread = information.GetThreadCount();
                telemetry.CpuTem = information.GetTemp();
                telemetry.Processes = information.GetProcesesCount();
                //Ram
                telemetry.Ram = information.GetRamAvailable();
                telemetry.RamInUse = information.GetRamInUse();
                telemetry.RamCache = information.GetRamCache();
                telemetry.RamCommitted = information.GetRamCommitted();
                telemetry.PagedPool = information.GetPagedPool();
                telemetry.NonPagedPool = information.GetNonPagedPool();
                //Disk
                telemetry.DiskReadTime = information.GetDiskReadTime();
                telemetry.DiskWriteTime = information.GetDiskWriteTime();
                telemetry.DiskReadBytes = information.GetReadBytes();
                telemetry.DiskWriteBytes = information.GetWriteBytes();
                telemetry.FreeMB = information.GetFreeMB();
                telemetry.FreeSpace = information.GetFreeSpace();
                telemetry.IdleTime = information.GetIdleTime();
                telemetry.DiskTime = information.GetDiskTime();
                //Wifi
                telemetry.DownloadRate = information.GetDownload();
                telemetry.UploadRate = information.GetUpload();
                telemetry.Bandwidth = information.GetBandwidth();
                telemetry.PacketsReceived = information.GetPacketsReceived();
                telemetry.PacketsSent = information.GetPacketsSent();
                telemetry.Packets = information.GetPackets();
                telemetry.ListTest = information.GetProcesses();

                var messageString = JsonConvert.SerializeObject(telemetry);
                Message message = new Message(Encoding.ASCII.GetBytes(messageString));
                await deviceClient.SendEventAsync(message);

                //Printing out message
                Console.WriteLine(messageString);
                
                Task.Delay(650).Wait();
            }
        }

        private async void ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (!kill)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                string JsonString = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                string newString = JsonString.Substring(2);
                command = JsonConvert.DeserializeObject<CommandDatapoint>(newString);

                ExacuteCommand();
                await deviceClient.CompleteAsync(receivedMessage);
            }
        }

        public static void ExacuteCommand()
        {
            //If the command has not expired
            if (command.ExpireTime.AddMinutes(1) > DateTime.Now)
            {
                if (command.CommandType == CommandType.CloseProcess)
                {
                    StopProcess(command.ProcessName);
                }
                else if (command.CommandType == CommandType.Hibernate)
                {
                    //Hibernates the computer
                    System.Diagnostics.Debug.WriteLine("Command Received:" + command);
                    SetSuspendState(true, true, true);
                }
                else if (command.CommandType == CommandType.ShutDown)
                {
                    System.Diagnostics.Debug.WriteLine("Command Received:" + command);
                    Process.Start("shutdown", "/s /t 0");
                    // starts the shutdown application                                      
                    // the argument /s is to shut down the computer                                   
                    // the argument /t 0 is to tell the process that                              
                    // the specified operation needs to be completed 
                    // after 0 seconds
                }
                else if (command.CommandType == CommandType.Restart)
                {
                    System.Diagnostics.Debug.WriteLine("Command Received:" + command);
                    Process.Start("shutdown", "/r /t 0"); // the argument /r is to restart the computer
                }
                else if (command.CommandType == CommandType.Logoff)
                {
                    System.Diagnostics.Debug.WriteLine("Command Received:" + command);
                    ExitWindowsEx(0, 0);
                }
                else if (command.CommandType == CommandType.Sleep)
                {
                    //Puts the computer into sleep mode
                    System.Diagnostics.Debug.WriteLine("Command Received:" + command);
                    SetSuspendState(false, true, true);
                }
                else if (command.CommandType == CommandType.Lock)
                {
                    System.Diagnostics.Debug.WriteLine("Lock:" + command);
                    LockWorkStation();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Command Expired" + newString + DateTime.Now);
            }
        }


        public static void StopProcess(string processName)
        {
            //Kills a process if it is found
            Process[] listOfProcesses = Process.GetProcesses();
            for (int i = 0; i < listOfProcesses.Length; i++)
            {
                if (listOfProcesses[i].ProcessName == processName)
                {
                    try
                    {
                        listOfProcesses[i].Kill();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                }
            }
        }

        public async void stop()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).list.IsEnabled = true;
            kill = true;
            worker.Dispose();
            await deviceClient.CloseAsync();
            deviceClient.Dispose();
        }

        public async void Test()
        {
            string a =  ((MainWindow) System.Windows.Application.Current.MainWindow).list.SelectedItem.ToString();
            ((MainWindow) System.Windows.Application.Current.MainWindow).list.IsEnabled = false;
            device = a.Replace("System.Windows.Controls.ListBoxItem: ", "");

            System.Diagnostics.Debug.WriteLine(""+device);
            kill = false;
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(device, deviceKey));

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_send;
            worker.RunWorkerAsync();
            ReceiveC2dAsync();
            Console.ReadLine();
        }
        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        [DllImport("user32")]
        public static extern void LockWorkStation();

        // Standby
        void worker_send(object sender, DoWorkEventArgs e)
        {
            SendDeviceToCloudMessagesAsync();
        }
    }


}
//Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

