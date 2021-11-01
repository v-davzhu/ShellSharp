using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ShellSharp.Client.Data;
using ShellSharp.Core;

namespace ShellSharp.Client
{

    public static class Program
    {
        private static ReverseTcpShellListener _listener;

        public static async Task Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);

            //// Add services to the container.
            //builder.Services.AddRazorPages();
            //builder.Services.AddServerSideBlazor();
            //builder.Services.AddSingleton<WeatherForecastService>();

            //var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Error");
            //}


            //app.UseStaticFiles();

            //app.UseRouting();


            //app.MapBlazorHub();
            //app.MapFallbackToPage("/_Host");

            //app.Run();

            Test();

            string command;
            while ((command = GetCommand()) != "quit")
            {
                if (!string.IsNullOrWhiteSpace(command))
                {
                    _listener.SendCommand(command);
                    Console.WriteLine(_listener.GetLastAnswer());
                }
            }
            _listener.Stop();
        }

        private static string? GetCommand()
        {
            Console.Write("cmd> ");
            return Console.ReadLine();
        }

        private static void Test() 
        {
            _listener = new ReverseTcpShellListener(4444);
            _listener.Start();
        }
    }
}