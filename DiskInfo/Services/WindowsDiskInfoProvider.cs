namespace DiskInfo.Services;

using DiskInfo.Models;

using HardwareInfo.Disk;

public sealed class WindowsDiskInfoProvider : IDiskInfoProvider
{
    public IReadOnlyList<DiskDisplayInfo> GetDisks()
    {
        var disks = new List<DiskDisplayInfo>();

        foreach (var disk in DiskInfo.GetInformation())
        {
            try
            {
                var displayInfo = CreateDisplayInfo(disk);
                if (displayInfo is not null)
                {
                    disks.Add(displayInfo);
                }
            }
            catch (IOException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (Win32Exception)
            {
            }
        }

        return disks
            .OrderBy(static x => x.TabTitle, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static DiskDisplayInfo? CreateDisplayInfo(IDiskInfo disk)
    {
        var smartValues = new List<SmartValueDisplayInfo>();

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

            smartValues.Add(new SmartValueDisplayInfo(1, nameof(nvme.CriticalWarning), nvme.CriticalWarning));
            smartValues.Add(new SmartValueDisplayInfo(2, nameof(nvme.Temperature), (ulong)nvme.Temperature));
            smartValues.Add(new SmartValueDisplayInfo(3, nameof(nvme.AvailableSpare), nvme.AvailableSpare));
            smartValues.Add(new SmartValueDisplayInfo(4, nameof(nvme.AvailableSpareThreshold), nvme.AvailableSpareThreshold));
            smartValues.Add(new SmartValueDisplayInfo(5, nameof(nvme.PercentageUsed), nvme.PercentageUsed));
            smartValues.Add(new SmartValueDisplayInfo(6, nameof(nvme.DataUnitRead), nvme.DataUnitRead));
            smartValues.Add(new SmartValueDisplayInfo(7, nameof(nvme.DataUnitWritten), nvme.DataUnitWritten));
            smartValues.Add(new SmartValueDisplayInfo(8, nameof(nvme.HostReadCommands), nvme.HostReadCommands));
            smartValues.Add(new SmartValueDisplayInfo(9, nameof(nvme.HostWriteCommands), nvme.HostWriteCommands));
            smartValues.Add(new SmartValueDisplayInfo(10, nameof(nvme.ControllerBusyTime), nvme.ControllerBusyTime));
            smartValues.Add(new SmartValueDisplayInfo(11, nameof(nvme.PowerCycles), nvme.PowerCycles));
            smartValues.Add(new SmartValueDisplayInfo(12, nameof(nvme.PowerOnHours), nvme.PowerOnHours));
            smartValues.Add(new SmartValueDisplayInfo(13, nameof(nvme.UnsafeShutdowns), nvme.UnsafeShutdowns));
            smartValues.Add(new SmartValueDisplayInfo(14, nameof(nvme.MediaErrors), nvme.MediaErrors));
            smartValues.Add(new SmartValueDisplayInfo(15, nameof(nvme.ErrorInfoLogEntries), nvme.ErrorInfoLogEntries));
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

                smartValues.Add(new SmartValueDisplayInfo((int)id, id.ToString(), attribute.Value.RawValue));

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

        if (smartValues.Count == 0)
        {
            return null;
        }

        var locations = disk.GetDrives()
            .Select(static x => x.Name)
            .Where(static x => !String.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var location = locations.Length == 0 ? disk.DeviceId : String.Join(", ", locations);

        return new DiskDisplayInfo(
            location,
            disk.Model,
            health,
            temperature,
            disk.FirmwareRevision,
            disk.BusType.ToString(),
            location,
            disk.Size,
            dataRead,
            dataWrite,
            powerCycles,
            powerOnHours,
            smartValues);
    }
}
