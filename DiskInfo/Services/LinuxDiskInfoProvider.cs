namespace DiskInfo.Services;

using DiskInfo.Models;

using LinuxDotNet.Disk;

public sealed class LinuxDiskInfoProvider : IDiskInfoProvider
{
    public IReadOnlyList<DiskData> GetDisks()
    {
        var disks = new List<DiskData>();

        foreach (var disk in DiskInfo.GetInformation())
        {
            var smartValues = new List<SmartValueData>();

            var health = default(int?);
            var temperature = default(int?);
            var dataRead = default(ulong?);
            var dataWrite = default(ulong?);
            var powerCycles = 0ul;
            var powerOnHours = 0ul;

            if (disk.SmartType == SmartType.Nvme && disk.Smart is ISmartNvme nvme)
            {
                health = 100 - nvme.PercentageUsed;
                temperature = nvme.Temperature;
                dataRead = nvme.DataUnitRead * 512 * 1000 / 1024 / 1024 / 1024;
                dataWrite = nvme.DataUnitWritten * 512 * 1000 / 1024 / 1024 / 1024;
                powerCycles = nvme.PowerCycles;
                powerOnHours = nvme.PowerOnHours;

                smartValues.Add(new SmartValueData(1, nameof(nvme.CriticalWarning), nvme.CriticalWarning));
                smartValues.Add(new SmartValueData(2, nameof(nvme.Temperature), (ulong)nvme.Temperature));
                smartValues.Add(new SmartValueData(3, nameof(nvme.AvailableSpare), nvme.AvailableSpare));
                smartValues.Add(new SmartValueData(4, nameof(nvme.AvailableSpareThreshold), nvme.AvailableSpareThreshold));
                smartValues.Add(new SmartValueData(5, nameof(nvme.PercentageUsed), nvme.PercentageUsed));
                smartValues.Add(new SmartValueData(6, nameof(nvme.DataUnitRead), nvme.DataUnitRead));
                smartValues.Add(new SmartValueData(7, nameof(nvme.DataUnitWritten), nvme.DataUnitWritten));
                smartValues.Add(new SmartValueData(8, nameof(nvme.HostReadCommands), nvme.HostReadCommands));
                smartValues.Add(new SmartValueData(9, nameof(nvme.HostWriteCommands), nvme.HostWriteCommands));
                smartValues.Add(new SmartValueData(10, nameof(nvme.ControllerBusyTime), nvme.ControllerBusyTime));
                smartValues.Add(new SmartValueData(11, nameof(nvme.PowerCycles), nvme.PowerCycles));
                smartValues.Add(new SmartValueData(12, nameof(nvme.PowerOnHours), nvme.PowerOnHours));
                smartValues.Add(new SmartValueData(13, nameof(nvme.UnsafeShutdowns), nvme.UnsafeShutdowns));
                smartValues.Add(new SmartValueData(14, nameof(nvme.MediaErrors), nvme.MediaErrors));
                smartValues.Add(new SmartValueData(15, nameof(nvme.ErrorInfoLogEntries), nvme.ErrorInfoLogEntries));
            }
            else if (disk.SmartType == SmartType.Generic && disk.Smart is ISmartGeneric generic)
            {
                foreach (var id in generic.GetSupportedIds())
                {
                    var attribute = generic.GetAttribute(id);
                    if (!attribute.HasValue)
                    {
                        continue;
                    }

                    smartValues.Add(new SmartValueData((int)id, id.ToString(), attribute.Value.RawValue));

                    switch (id)
                    {
                        case SmartId.Temperature:
                            temperature = (short)(attribute.Value.RawValue & 0xFF);
                            break;
                        case SmartId.PowerCycleCount:
                            powerCycles = attribute.Value.RawValue;
                            break;
                        case SmartId.PowerOnHours:
                            powerOnHours = attribute.Value.RawValue;
                            break;
                        case SmartId.PercentageLifetimeRemaining:
                            health = 100 - (int)attribute.Value.RawValue;
                            break;
                    }
                }
            }
            else
            {
                continue;
            }

            var mountPoints = disk.GetPartitions()
                .Select(static x => x.MountPoint)
                .Where(static x => !String.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var location = mountPoints.Length == 0 ? disk.DeviceName : String.Join(", ", mountPoints);

            disks.Add(new DiskData(
                location,
                disk.DiskType.ToString(),
                disk.Model,
                disk.FirmwareRevision,
                disk.Size,
                location,
                health,
                temperature,
                dataRead,
                dataWrite,
                powerCycles,
                powerOnHours,
                smartValues));
        }

        return disks
            .OrderBy(static x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
