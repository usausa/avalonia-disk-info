namespace DiskInfo.Models;

public sealed class SmartValueData
{
    public int Id { get; }

    public string? Name { get; }

    public ulong RawValue { get; }

    public SmartValueData(int id, string? name, ulong rawValue)
    {
        Id = id;
        Name = name;
        RawValue = rawValue;
    }
}
