using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.IO;

[assembly: OwinStartup(typeof(StartupDemo.ProductionStartup))]

namespace StartupDemo
{
    public class ProductionStartup
    {
        public void Configuration(IAppBuilder app)
        {
            /* app.Run(context =>
             {
                 string t = DateTime.Now.Millisecond.ToString();
                 return context.Response.WriteAsync(t + "Production OWIN APP");
             });
             */
            /*app.Run(context =>
            {
                OAuthClientTest oAuthClientTest = new OAuthClientTest();
                oAuthClientTest.Get_Accesss_Token_By_Client_Credentials_Grant();
            }); */
        }
    }

#if RELEASE
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 有关如何配置应用程序的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=316888

            /*app.Use拉姆达表达式注册指定的中间件到OWIN管道。
            在这个例子中，我们在响应传入请求前建立传入请求的日志记录。
            next参数是管道中下一个组件的委托。
            app.Run拉姆达表达式将管道挂钩到传入请求并提供响应机制。
            注意：在上面的代码，注释掉了OwinStartup特性，我们依赖于运行名为Startup的类这一约定。
            */
            app.Use((context, next) =>
            {
                TextWriter output = context.Get<TextWriter>("host.TraceOutput");
                return next().ContinueWith(result =>
                {
                    output.WriteLine("Scheme {0} :Method {1} :Path {2} :MS {3}",
                        context.Request.Scheme,
                        context.Request.Method,
                        context.Request.Path,
                        DateTime.Now.Millisecond.ToString());

                });
            });
            app.Run(async context =>
            {
                await context.Response.WriteAsync(DateTime.Now.Millisecond.ToString() + "My First OWIN App");
            });
        }
    }
#endif
}
