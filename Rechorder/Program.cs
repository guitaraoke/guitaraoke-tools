using Microsoft.Extensions.FileProviders;

public class Program {
    public static string RootPath = @"C:\Users\dylan\dropbox\creative\guitaraoke\tagged";
    public static void Main(string[] args) {


        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDirectoryBrowser();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        var paths = new[] {
    @"C:\Users\dylan\Dropbox\creative\guitaraoke\tagged",
    @"D:\Dropbox\creative\guitaraoke\tagged",
    @"/Users/Dylan/Dropbox/Creative/Guitaraoke/tagged"
};
        PhysicalFileProvider FileProvider = null;
        foreach (var path in paths) {
            if (Directory.Exists(path)) {
                FileProvider = new PhysicalFileProvider(path);
                RootPath = path;    
                break;
            }
        }
        if (FileProvider == null) throw new Exception("Could not find tagged directory");

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