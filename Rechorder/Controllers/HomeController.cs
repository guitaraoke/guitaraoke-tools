using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Rechorder.Models;
using System.IO;

namespace Rechorder.Controllers;

public class HomeController : Controller {
    string RootPath = Program.RootPath;
    private readonly ILogger<HomeController> logger;

    public HomeController(ILogger<HomeController> logger) {
        this.logger = logger;
    }

    public IActionResult Index(string file = "") {
        var finishedFilePath = Directory.GetParent(RootPath).FullName;
        var finishedFiles = Directory
            .GetFiles(Path.Combine(finishedFilePath, "5 Output"))
            .Select(f => Path.GetFileName(f));           ;
        foreach(var ff in finishedFiles) logger.LogInformation(ff);

        logger.LogInformation(file);
        var mixes = Directory
            .GetFiles(Path.Combine(RootPath, "2 Mixes"))
            .Where(file => file.EndsWith(".mp4"))
            .Where(file => ! finishedFiles.Contains(Path.GetFileName(file).Replace(".mp4", " - with chords.mp4")))            
            .Select(file => new FileInfo(file))
            .OrderBy(f => f.Name);

        var chordNamesFilePath = GetChordNamesFilePath(file);
        logger.LogInformation(chordNamesFilePath);
        Console.WriteLine(chordNamesFilePath);
        var chordNames = System.IO.File.Exists(chordNamesFilePath) ? System.IO.File.ReadAllText(chordNamesFilePath) : 
            $"{chordNamesFilePath} does not exist"
        ;

        var chordTimesFilePath = GetChordTimesFilePath(file);
        var chordTimes = System.IO.File.Exists(chordTimesFilePath) ? System.IO.File.ReadAllText(chordTimesFilePath) : "";
        
        var model = new DashboardViewModel {
            Files = mixes,
            SelectedFile = file,
            RootPath = RootPath,
            ChordNamesFilePath = Path.GetFullPath(chordNamesFilePath),
            ChordTimesFilePath = Path.GetFullPath(chordTimesFilePath),
            ChordNames = chordNames,
            ChordTimes = chordTimes 
        };

        return View(model);
    }

    private string GetChordNamesFilePath(string file) {
        return Path.Combine(RootPath, "3 Chords", Path.GetFileName(file).Replace(".mp4", "") + " - chord names.txt");
    }

    private string GetChordTimesFilePath(string file) {
        return Path.Combine(RootPath, "3 Chords", Path.GetFileName(file).Replace(".mp4", "") + " - chord times.txt");
    }

    public IActionResult Privacy() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public ActionResult ChordNames(string file, string chordNames) {
        var filePath = GetChordNamesFilePath(file);
        System.IO.File.WriteAllText(filePath, chordNames);
        return NoContent();
    }    
    public ActionResult ChordTimes(string file, string chordTimes) {
        var filePath = GetChordTimesFilePath(file);
        System.IO.File.WriteAllText(filePath, chordTimes);
        return NoContent();
    }

}
