namespace Rechorder.Models;

public class ErrorViewModel {
    public string? RequestId { get; set; }

    public bool ShowRequestId => !String.IsNullOrEmpty(RequestId);
}

public class DashboardViewModel {
    public IEnumerable<FileInfo> Files { get; set; } = new List<FileInfo>();
    public string SelectedFile { get; set; } = String.Empty;    
    public string ChordNames { get; set; } = String.Empty;
    public string ChordTimes { get; set; } = String.Empty;
    public string ChordNamesFilePath { get;  set; } = String.Empty;
    public string ChordTimesFilePath { get; set; } = String.Empty;

    public string RootPath { get; set; } = String.Empty;
}

