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
        logger.LogInformation(file);
        var files = Directory
            .GetFiles(RootPath)
            .Where(file => file.EndsWith(".mp4"))
            .Select(file => new FileInfo(file))
            .OrderBy(f => f.Name);

        var chordNamesFilePath = Path.Combine("chord-names", Path.GetFileName(file).Replace(".mp4", " - chord names.txt"));
        logger.LogInformation(chordNamesFilePath);
        var chordNames = System.IO.File.Exists(chordNamesFilePath) ? System.IO.File.ReadAllText(chordNamesFilePath) : "";

        var chordTimesFilePath = Path.Combine("chord-times", Path.GetFileName(file).Replace(".mp4", " - chord times.txt"));
        var chordTimes = System.IO.File.Exists(chordTimesFilePath) ? System.IO.File.ReadAllText(chordTimesFilePath) : "";
        
        var model = new DashboardViewModel {
            Files = files,
            SelectedFile = file,
            ChordNames = chordNames,
            ChordTimes = chordTimes 
        };

        return View(model);
    }

    public IActionResult Privacy() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public ActionResult ChordNames(string file, string chordNames) {
        var filePath = Path.Combine("chord-names", Path.GetFileName(file) + " - chord names.txt");
        System.IO.File.WriteAllText(filePath, chordNames);
        return NoContent();
    }    
    public ActionResult ChordTimes(string file, string chordTimes) {
        var filePath = Path.Combine("chord-times", Path.GetFileName(file) + " - chord times.txt");
        System.IO.File.WriteAllText(filePath, chordTimes);
        return NoContent();
    }

}
