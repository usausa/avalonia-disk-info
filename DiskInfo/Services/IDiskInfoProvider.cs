namespace DiskInfo.Services;

using DiskInfo.Models;

public interface IDiskInfoProvider
{
    IReadOnlyList<DiskDisplayInfo> GetDisks();
}
