using Meier.Services;

namespace Meier;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("请输入电影页面链接：");
        var url = Console.ReadLine();
        
        if (!string.IsNullOrEmpty(url))
        {
            var downloader = new MovieDownloader();
            await downloader.ParseUrl(url);
        }
        else
        {
            Console.WriteLine("链接不能为空！");
        }
    }
}
