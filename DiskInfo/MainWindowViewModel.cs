namespace DiskInfo;

using DiskInfo.Models;
using DiskInfo.Services;

public sealed class MainWindowViewModel
{
    public ObservableCollection<DiskData> Disks { get; }

    public MainWindowViewModel(IDiskInfoProvider provider)
    {
#pragma warning disable IDE0028
        Disks = new ObservableCollection<DiskData>(provider.GetDisks());
#pragma warning restore IDE0028
    }
}
