namespace DiskInfo.Models;

public sealed class SmartValueDisplayInfo
{
    public int Id { get; }

    public string? Name { get; }

    public ulong RawValue { get; }

    public SmartValueDisplayInfo(int id, string? name, ulong rawValue)
    {
        Id = id;
        Name = name;
        RawValue = rawValue;
    }
}
