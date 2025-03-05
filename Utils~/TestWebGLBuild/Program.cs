using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var directoryToServe = Directory.GetCurrentDirectory();
        int port = 5001;

        if (args.Length > 0)
        {
            directoryToServe = Path.GetFullPath(args[0]);
        }

        Console.WriteLine("Serving WebGL build from "+ directoryToServe);

        var host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel()
                          .UseContentRoot(directoryToServe)
                          .ConfigureKestrel(serverOptions =>
                          {
                              serverOptions.ListenAnyIP(port, listenOptions =>
                              {
                                  listenOptions.UseHttps();
                              });
                          })
                          .Configure(app =>
                          {
                              app.Use(async (context, next) =>
                              {
                                  // Check if the request is for a .bz file
                                  if (context.Request.Path.Value.EndsWith(".br", StringComparison.OrdinalIgnoreCase))
                                  {
                                      // Set the Content-Encoding header to indicate Bzip2 compression
                                      context.Response.Headers["Content-Encoding"] = "br";
                                  }
                                  if (context.Request.Path.Value.EndsWith(".wasm.br", StringComparison.OrdinalIgnoreCase))
                                  {
                                      // Set the Content-Encoding header to indicate Bzip2 compression
                                      context.Response.Headers["Content-Type"] = "application/wasm";
                                  }

                                  await next();
                              });

                              app.UseDefaultFiles();

                              app.UseStaticFiles(new StaticFileOptions
                              {
                                  FileProvider = new PhysicalFileProvider(directoryToServe),
                                  ServeUnknownFileTypes = true // To serve files without known MIME types
                              });
                          });
            })
            .Build();

        Process.Start(new ProcessStartInfo
        {
            FileName = $"https://localhost:{port}/index.html",
            UseShellExecute = true
        });

        host.Run();
    }
}