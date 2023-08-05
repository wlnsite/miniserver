using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wlniao;
var cfg = XCore.Init(args);
if (cfg != null)
{
	var http = new Http { ListenPort = cfg.GetInt32("Port", 80) };
	http.Handler = new System.Action<Http.Context>((ctx) =>
	{
		if (ctx.Path == "/")
		{
			ctx.Path = "/index.html";
		}
		var reqFile = System.IO.Path.Combine(Wlniao.XCore.StartupRoot, ctx.Path.TrimStart('/'));
		if (File.Exists(reqFile))
		{
			//文件已存在时，直接输出文件
			ctx.ContentType = MimeMapping.GetMimeMapping(reqFile);
			ctx.Response = File.ReadAllBytes(reqFile);
		}
		else
		{
			var list = "";
			var html = "<html><head><title>MiniWeb</title></head><body><ul>{{content}}</ul></body></html>";
			foreach (var item in System.IO.Directory.GetFiles(Wlniao.XCore.StartupRoot, "*", SearchOption.AllDirectories))
			{
				var path = System.IO.Path.GetRelativePath(Wlniao.XCore.StartupRoot, item);
				if (path.StartsWith("."))
				{
					continue;
				}
				list += "<li><a href=\"/" + path + "\">" + path + "</a></li>";
			}
			ctx.Response = html.Replace("{{content}}", list);
			ctx.ContentType = "text/html";
		}
	});
	http.Start();
}