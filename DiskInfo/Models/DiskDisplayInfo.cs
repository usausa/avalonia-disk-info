namespace DiskInfo.Models;

public sealed class DiskDisplayInfo
{
    public string? TabTitle { get; }

    public string? Model { get; }

    public int? Health { get; }

    public int? Temperature { get; }

    public string? FirmwareRevision { get; }

    public string? BusType { get; }

    public string? Location { get; }

    public ulong Size { get; }

    public ulong? DataReadGigaBytes { get; }

    public ulong? DataWriteGigaBytes { get; }

    public ulong PowerCycles { get; }

    public ulong PowerOnHours { get; }

    public IReadOnlyList<SmartValueDisplayInfo> SmartValues { get; }

    public DiskDisplayInfo(
        string? tabTitle,
        string? model,
        int? health,
        int? temperature,
        string? firmwareRevision,
        string? busType,
        string? location,
        ulong size,
        ulong? dataReadGigaBytes,
        ulong? dataWriteGigaBytes,
        ulong powerCycles,
        ulong powerOnHours,
        IReadOnlyList<SmartValueDisplayInfo> smartValues)
    {
        TabTitle = tabTitle;
        Model = model;
        Health = health;
        Temperature = temperature;
        FirmwareRevision = firmwareRevision;
        BusType = busType;
        Location = location;
        Size = size;
        DataReadGigaBytes = dataReadGigaBytes;
        DataWriteGigaBytes = dataWriteGigaBytes;
        PowerCycles = powerCycles;
        PowerOnHours = powerOnHours;
        SmartValues = smartValues;
    }
}
