namespace DiskInfo.Models;

public sealed class DiskData
{
    public string? Name { get; }

    public string? BusType { get; }

    public string? Model { get; }

    public string? FirmwareRevision { get; }

    public ulong Size { get; }

    public string? Location { get; }

    public int? Health { get; }

    public int? Temperature { get; }

    public ulong? DataReadGigaBytes { get; }

    public ulong? DataWriteGigaBytes { get; }

    public ulong PowerCycles { get; }

    public ulong PowerOnHours { get; }

    public IReadOnlyList<SmartValueData> SmartValues { get; }

    public DiskData(
        string? name,
        string? busType,
        string? model,
        string? firmwareRevision,
        ulong size,
        string? location,
        int? health,
        int? temperature,
        ulong? dataReadGigaBytes,
        ulong? dataWriteGigaBytes,
        ulong powerCycles,
        ulong powerOnHours,
        IReadOnlyList<SmartValueData> smartValues)
    {
        Name = name;
        BusType = busType;
        Model = model;
        FirmwareRevision = firmwareRevision;
        Size = size;
        Location = location;
        Health = health;
        Temperature = temperature;
        DataReadGigaBytes = dataReadGigaBytes;
        DataWriteGigaBytes = dataWriteGigaBytes;
        PowerCycles = powerCycles;
        PowerOnHours = powerOnHours;
        SmartValues = smartValues;
    }
}
