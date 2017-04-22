using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1;

namespace Agent._4Services
{
    class PerformanceCounters
    {
        private PerformanceCounter cpu;
        private PerformanceCounter cpu2;
        private PerformanceCounter thread;
        private PerformanceCounter cpuTem;
        private PerformanceCounter processes;
        private PerformanceCounter percent;

        private PerformanceCounter ram;
        private PerformanceCounter ramInUse;
        private PerformanceCounter ramCache;
        private PerformanceCounter ramCommitted;
        private PerformanceCounter pagedPool;
        private PerformanceCounter nonPagedPool;

        private PerformanceCounter diskReadTime;
        private PerformanceCounter diskWriteTime;
        private PerformanceCounter diskReadBytes;
        private PerformanceCounter diskWriteBytes;
        private PerformanceCounter freeMB;
        private PerformanceCounter freeSpace;
        private PerformanceCounter idleTime;
        private PerformanceCounter diskTime;


        List<PerformanceCounter> downloadList;
        List<PerformanceCounter> uploadList;
        List<PerformanceCounter> bandwidthList;
        List<PerformanceCounter> packetsReceivedList;
        List<PerformanceCounter> packetsSentList;
        List<PerformanceCounter> packetsList;
        private PerformanceCounterCategory performanceCounterCategory;

        Process[] listOfProcesses = Process.GetProcesses();
        public PerformanceCounters()
        {
            //CPU
            cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpu2 = new PerformanceCounter("Processor Information", "Processor Frequency", "_Total");
            thread = new PerformanceCounter("Process", "Thread Count", "_Total");
            cpuTem = new PerformanceCounter("Thermal Zone Information", "Temperature", "\\_TZ.TZ01");
            processes = new PerformanceCounter("System", "Processes");
            percent = new PerformanceCounter("Processor Information", "% of Maximum Frequency", "_Total");
            //Ram
            ram = new PerformanceCounter("Memory", "Available MBytes", true);
            ramInUse = new PerformanceCounter("Memory", "% Committed Bytes In Use", true);
            ramCache = new PerformanceCounter("Memory", "Cache Bytes", true);
            ramCommitted = new PerformanceCounter("Memory", "Committed Bytes", true);
            pagedPool = new PerformanceCounter("Memory", "Pool Paged Bytes", true);
            nonPagedPool = new PerformanceCounter("Memory", "Pool Nonpaged Bytes", true);
            //Disk
            diskReadTime = new PerformanceCounter("LogicalDisk", "% Disk Read Time", "_Total");
            diskWriteTime = new PerformanceCounter("LogicalDisk", "% Disk Write Time", "_Total");
            diskReadBytes = new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", "_Total");
            diskWriteBytes = new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", "_Total");
            freeMB = new PerformanceCounter("LogicalDisk", "Free Megabytes", "_Total");
            freeSpace = new PerformanceCounter("LogicalDisk", "% Free Space", "_Total");
            idleTime = new PerformanceCounter("LogicalDisk", "% Idle Time", "_Total");
            diskTime = new PerformanceCounter("LogicalDisk", "% Disk Time", "_Total");
            //WIFI
            performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
            //TODO Should not hardcode 16/2/17

            String[] instancename = performanceCounterCategory.GetInstanceNames();
            downloadList = new List<PerformanceCounter>();
            uploadList = new List<PerformanceCounter>();
            bandwidthList = new List<PerformanceCounter>();
            packetsReceivedList = new List<PerformanceCounter>();
            packetsSentList = new List<PerformanceCounter>();
            packetsList = new List<PerformanceCounter>();

            foreach (string name in instancename)
            {
                downloadList.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", name));
                uploadList.Add(new PerformanceCounter("Network Interface", "Bytes Sent/sec", name));
                bandwidthList.Add(new PerformanceCounter("Network Interface", "Bytes Total/sec", name));
                packetsReceivedList.Add(new PerformanceCounter("Network Interface", "Packets Received/sec", name));
                packetsSentList.Add(new PerformanceCounter("Network Interface", "Packets Sent/sec", name));
                packetsList.Add(new PerformanceCounter("Network Interface", "Packets/sec", name)); ;
            }
        }

        private TimeSpan time;
        string date = "";

        private List<ProcessData> myList;
        public List<ProcessData> GetProcesses()
        {
            listOfProcesses = Process.GetProcesses();
            myList = new List<ProcessData>();
            foreach (var process in listOfProcesses)
            {
                try
                {
                    if (process.ProcessName != "Idle")
                    {
                        if (process.StartTime.Day == DateTime.Today.Day)
                        {
                            date = "" + process.StartTime.Hour + ":" + process.StartTime.Minute + ":" +
                                   process.StartTime.Second;
                        }
                        else
                        {
                            date = "" + process.StartTime;
                        }
                        myList.Add(new ProcessData()
                        {
                            N = process.ProcessName,
                            M = "" + process.NonpagedSystemMemorySize64,
                            C = "",
                            T = date,
                        });
                    }
                }
                catch (Exception e)
                {
                    
                }
            }
            return myList;
        }
        public string GetFrequency()
        {
            //for (int i = 0; i < performanceCounterCategory.GetInstanceNames().Length; i++)
            //{
            //    Console.WriteLine("www" + performanceCounterCategory.GetInstanceNames()[i]);
            //}
            return cpu2.NextValue().ToString();
        }

        public string GetPackets()
        {
            double packetsTotal = 0;

            foreach (var VARIABLE in packetsList)
            {
                packetsTotal  += Convert.ToDouble(VARIABLE.NextValue());
            }
            packetsTotal = Math.Round((packetsTotal), 1);
            return packetsTotal.ToString();
        }

        public string GetPacketsReceived()
        {
            double packets = 0;

            foreach (var VARIABLE in packetsReceivedList)
            {
                packets += Convert.ToDouble(VARIABLE.NextValue());
            }
            packets = Math.Round((packets), 1);
            return packets.ToString();
        }
        public string GetPacketsSent()
        {
            double packets = 0;
            foreach (var VARIABLE in packetsSentList)
            {
                packets += Convert.ToDouble(VARIABLE.NextValue());
            }
            packets = Math.Round((packets), 1);
            return packets.ToString();
        }

        public string GetBandwidth()
        {
            double band = 0;
            foreach (var VARIABLE in bandwidthList)
            {
                band += Convert.ToDouble(VARIABLE.NextValue());
            }
            band = Math.Round((band / 1000000), 3);
            return band.ToString();
        }

        public string GetUpload()
        {
            double upload = 0;
            foreach (var VARIABLE in uploadList)
            {
                upload += Convert.ToDouble(VARIABLE.NextValue());
            }
            upload = Math.Round((upload / 1000000), 3);
            return upload.ToString();
        }

        public string GetDownload()
        {
            double download = 0;
            foreach (var VARIABLE in downloadList)
            {
                download += Convert.ToDouble(VARIABLE.NextValue());
            }

            download = Math.Round((download / 1000000), 3);
            return download.ToString();
        }

        public string GetPercent()
        {
            double percent = Convert.ToDouble(this.percent.NextValue());
            percent = Math.Round(percent, 2);
            return percent.ToString();
        }

        public string GetDiskTime()
        {
            double percent = Convert.ToDouble(diskTime.NextValue());
            percent = Math.Round(percent, 2);
            return percent.ToString();
        }

        public string GetIdleTime()
        {
            double percent = Convert.ToDouble(idleTime.NextValue());
            percent = Math.Round(percent, 2);
            return percent.ToString();
        }

        public string GetFreeSpace()
        {
            double percent = Convert.ToDouble(freeSpace.NextValue());
            percent = Math.Round(percent, 2);
            return percent.ToString();
        }

        public string GetFreeMB()
        {
            double read = Convert.ToDouble(freeMB.NextValue());
            read = Math.Round((read / 1000), 3);
            return read.ToString();
        }


        public string GetWriteBytes()
        {
            double read = Convert.ToDouble(diskWriteBytes.NextValue());
            read = Math.Round((read / 1000000), 3);
            return read.ToString();
        }

        public string GetReadBytes()
        {
            double write = Convert.ToDouble(diskReadBytes.NextValue());
            write = Math.Round((write / 1000000), 3);
            return write.ToString();
        }

        public string GetDiskWriteTime()
        {
            double percent = Convert.ToDouble(diskWriteTime.NextValue());
            percent = Math.Round(percent, 2);
            return percent.ToString();
        }

        public string GetDiskReadTime()
        {
            double percent = Convert.ToDouble(diskReadTime.NextValue());
            percent = Math.Round(percent, 2);
            return percent.ToString();
        }

        public string GetNonPagedPool()
        {
            double npp = Convert.ToDouble(nonPagedPool.NextValue());
            npp = Math.Round((npp / 1073741824), 3);
            return npp.ToString();
        }

        public string GetPagedPool()
        {
            double pPaged = Convert.ToDouble(pagedPool.NextValue());
            pPaged = Math.Round((pPaged / 1073741824), 3);
            return pPaged.ToString();
        }

        public string GetRamCommitted()
        {
            double committed = Convert.ToDouble(ramCommitted.NextValue());
            committed = Math.Round((committed / 1073741824), 3);
            return committed.ToString();
        }

        public string GetRamCache()
        {
            //1,073,741,824
            double cache = Convert.ToDouble(ramCache.NextValue());
            cache = Math.Round((cache / 107374182), 3); //removed 4
            return cache.ToString();
        }

        public string GetRamInUse()
        {
            double percent = Convert.ToDouble(ramInUse.NextValue());
            percent = Math.Round(percent, 2);
            return percent.ToString();
        }

        public string GetRamAvailable()
        {
            return ram.NextValue().ToString();
        }

        public string GetTemp()
        {
            double celcius = System.Convert.ToDouble(cpuTem.NextValue());
            celcius -= 273.15;
            return celcius.ToString();
        }

        public string GetCPU()
        {
            double cpuPercent = Convert.ToDouble(cpu.NextValue());
            cpuPercent = Math.Round(cpuPercent, 3);
            return cpuPercent.ToString();
        }

        public string GetThreadCount()
        {
            return thread.NextValue().ToString();
        }

        public string GetProcesesCount()
        {
            return processes.NextValue().ToString();
        }
    }
}
