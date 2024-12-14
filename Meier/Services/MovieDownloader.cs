using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Meier.Services
{
    public class MovieDownloader
    {
        private readonly List<(string Name, string Url, string Size)> _downloadLinks = new();

        public async Task ParseUrl(string url)
        {
            try
            {
                using var client = new HttpClient();
                var content = await client.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(content);

                // 查找所有下载链接
                var linkNodes = doc.DocumentNode.SelectNodes("//div[@class='bf8243b9']//li//div");
                if (linkNodes != null)
                {
                    foreach (var node in linkNodes)
                    {
                        var linkElement = node.SelectSingleNode(".//a[@class='baf6e960dd']");
                        if (linkElement != null)
                        {
                            var name = linkElement.InnerText.Trim();
                            var downloadUrl = linkElement.GetAttributeValue("href", "");
                            var sizeMatch = Regex.Match(name, @"<span>(.*?)</span>");
                            var size = sizeMatch.Success ? sizeMatch.Groups[1].Value : "";

                            // 清理文件名中的大小信息
                            name = Regex.Replace(name, @"\s*<span>.*?</span>", "").Trim();

                            _downloadLinks.Add((name, downloadUrl, size));
                        }
                    }
                }

                DisplayDownloadOptions();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误：{ex.Message}");
            }
        }

        private void DisplayDownloadOptions()
        {
            if (_downloadLinks.Count == 0)
            {
                Console.WriteLine("\n未找到下载链接！");
                return;
            }

            Console.WriteLine("\n可用下载选项：");
            for (int i = 0; i < _downloadLinks.Count; i++)
            {
                var (name, _, size) = _downloadLinks[i];
                Console.WriteLine($"{i + 1}. {name} {(string.IsNullOrEmpty(size) ? "" : $"[{size}]")}");
            }

            Console.Write($"\n请输入序号选择要下载的版本 (1-{_downloadLinks.Count}): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= _downloadLinks.Count)
            {
                var selected = _downloadLinks[choice - 1];
                Console.WriteLine($"\n您选择了：{selected.Name} {(string.IsNullOrEmpty(selected.Size) ? "" : $"[{selected.Size}]")}");
                Console.WriteLine($"下载链接：{selected.Url}");
            }
            else
            {
                Console.WriteLine("无效的选择！");
            }
        }
    }
}