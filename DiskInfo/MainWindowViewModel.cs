namespace DiskInfo;

using DiskInfo.Models;
using DiskInfo.Services;

public sealed class MainWindowViewModel
{
    public ObservableCollection<DiskDisplayInfo> Disks { get; }

    public bool HasDisks => Disks.Count > 0;

    public bool HasNoDisks => Disks.Count == 0;

    public MainWindowViewModel(IDiskInfoProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        Disks = new ObservableCollection<DiskDisplayInfo>(provider.GetDisks());
    }
}
