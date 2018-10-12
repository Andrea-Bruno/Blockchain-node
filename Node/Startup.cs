using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
namespace Node
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
      System.Net.ServicePointManager.DefaultConnectionLimit = 10;
      String MyAddress = null;
      if (env.IsDevelopment())
      {
#if DEBUG
        MyAddress = "http://localhost:55007";
#elif DEBUGNETWORK
        MyAddress = "http://localhost:55008";
#endif
      }
      var Network = BlockchainManager.HookToNetwork.Initialize(MyAddress);
      //NetworkManager.Network.Initialize();
      Network.Test();
      app.Run(async (context) =>
      {
        if (context.Request.Method == "POST")
        {
          var QueryString = new NameValueCollection();
          foreach (var item in context.Request.Query)
            QueryString.Add(item.Key, item.Value);
          var Form = new NameValueCollection();
          foreach (var item in context.Request.Form)
            Form.Add(item.Key, item.Value);
          using (System.IO.Stream Stream = new System.IO.MemoryStream())
            if (NetworkManager.Network.OnReceivesHttpRequest(QueryString, Form, context.Connection.RemoteIpAddress.ToString(), out string ContentType, Stream))
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
