using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Agent._4Services;
using ConsoleApplication1;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace MyDeviceTest
{
    class DeviceTest
    {
        static DeviceClient deviceClient;
        private BackgroundWorker worker;
        private bool kill = false;
        private static string iotHubUri = "ManageIoT.azure-devices.net";
        static string deviceKey = "lH5w0GYV8xxACr7lqeB//x9uu0nJVHhC7tRVFr+HF2I=";
        //lH5w0GYV8xxACr7lqeB//x9uu0nJVHhC7tRVFr+HF2I=
        //LFqCq06kB8m/7abwakvP1tBjo8zLWATyN3soGLogOew= device2
        PerformanceCounters information;
        private TelemetryDatapoint telemetry;
        //Sending messages to the IoT hub
        private async Task SendDeviceToCloudMessagesAsync()
        {
            information = new PerformanceCounters();

            //Loop for sending messages
            while (!kill)
            {
                //Creating a telemetry object, setting data, sending over Iot Azure
                telemetry = new TelemetryDatapoint("Device_1");
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
                
                Task.Delay(450).Wait();
            }
        }

        public async void stop()
        {
            kill = true;
            worker.Dispose();
            await deviceClient.CloseAsync();
            deviceClient.Dispose();
        }

        public async void Test()
        {
            kill = false;
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("Device_1", deviceKey));
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_send;
            worker.RunWorkerAsync();
            Console.ReadLine();
        }
        void worker_send(object sender, DoWorkEventArgs e)
        {
            SendDeviceToCloudMessagesAsync();
        }
    }


}
//Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

