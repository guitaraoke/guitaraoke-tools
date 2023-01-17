using Microsoft.Extensions.FileProviders;

public class Program {
    public static string RootPath = @"C:\Users\dylan\dropbox\creative\guitaraoke\tagged";
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();
        builder.Services.AddDirectoryBrowser();
        var app = builder.Build();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        var paths = new[] {
            @"C:\Users\dylan\Dropbox\creative\guitaraoke\",
            @"D:\Dropbox\creative\guitaraoke\",
            @"/Users/Dylan/Dropbox/Creative/Guitaraoke/"
        };

        PhysicalFileProvider FileProvider = null;
        foreach (var path in paths) {
            var mixesDirectoryName = Path.Combine(path, "2 Mixes");
            if (Directory.Exists(path)) {
                FileProvider = new PhysicalFileProvider(mixesDirectoryName);
                RootPath = path;
                break;
            }
        }
        if (FileProvider == null) throw new Exception("Could not find Guitaraoke directory");

        var RequestPath = new PathString("/videos");
        app.UseStaticFiles(new StaticFileOptions() {
            FileProvider = FileProvider,
            RequestPath = RequestPath
        });
        app.UseDirectoryBrowser(new DirectoryBrowserOptions {
            FileProvider = FileProvider,
            RequestPath = RequestPath
        });

        app.UseRouting();
        app.UseAuthorization();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.Run();
    }
}