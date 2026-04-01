namespace DiskInfo;

using DiskInfo.Models;
using DiskInfo.Services;

public sealed class MainWindowViewModel
{
    public ObservableCollection<DiskData> Disks { get; }

    public MainWindowViewModel(IDiskInfoProvider provider)
    {
        Disks = new ObservableCollection<DiskData>(provider.GetDisks());
    }
}
