namespace DiskInfo;

using DiskInfo.Models;
using DiskInfo.Services;

public sealed class MainWindowViewModel
{
    public ObservableCollection<DiskDisplayInfo> Disks { get; }

    public MainWindowViewModel(IDiskInfoProvider provider)
    {
        Disks = new ObservableCollection<DiskDisplayInfo>(provider.GetDisks());
    }
}
