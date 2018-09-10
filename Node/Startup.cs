using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using static NetworkManager.Network;
namespace Network
{
  public class Startup
  {
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      BlockchainManager.HookToNetwork.Initialize(NetworkManager.Setup.Network.MyAddress);
      //NetworkManager.Network.Initialize();
      app.Run(async (context) =>
      {
        var Request = context.Request;
        if (Request.Method == "POST")
        {
          var QueryString = Request.Query.ToDictionary(d => d.Key, d => d.Value.ToString());
          var Form = Request.Form.ToDictionary(d => d.Key, d => d.Value.ToString());
          using (System.IO.Stream Stream = new System.IO.MemoryStream())
            if (NetworkManager.Network.OnReceivesHttpRequest(QueryString, Form, out string ContentType, Stream))
            {
              context.Response.ContentType = ContentType;
              Stream.Position = 0;
              using (var streamReader = new System.IO.StreamReader(Stream))
                await context.Response.WriteAsync(streamReader.ReadToEnd());
              return;
            }
        }
        await context.Response.WriteAsync("Online!");
      });
    }
  }
}
