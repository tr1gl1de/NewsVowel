using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;

namespace NewsVowelTask;

public static class NewsParseExtensions
{
    private static string GetNewsValue(Article article, string fieldName) =>
        fieldName switch
        {
            "author" => article.Author,
            "title" => article.Title,
            "content" => article.Content,
            "description" => article.Description,
            _ => article.Content
        };

    public static async Task<IList<Article>> ParseNews(string key, string theme, int pageNum, int pageSize)
    {
        var newsApiClient = new NewsApiClient(key);
        var news = await newsApiClient.GetEverythingAsync(new EverythingRequest
        {
            Q = theme,
            SortBy = SortBys.Popularity,
            PageSize = pageSize,
            Page = pageNum
        });

        return news.Articles;
    }

    public static IReadOnlyDictionary<string, string> GetVowelsFromNews(this IEnumerable<Article> news,
        string fieldName)
    {
        var newsContentWithMaxVowelsWords = new Dictionary<string, string>();
        var enVowels = new[] { 'a', 'e', 'i', 'o', 'u' };
        var ruVowels = new[] { 'а', 'э', 'ы','о','у','я','е','и','ё','ю'};
        var vowels = new HashSet<char>();
        vowels.UnionWith(enVowels);
        vowels.UnionWith(ruVowels);
        var delimiters = new[]
            {
                ' ', ',', '.', '`', ';', '/', '\'', ':', '{', '}', '[', ']', '\\', '|', '+', '(', ')', '\t',
                '\r', '\n', '?', '!', '"', '<', '>'
            };

        var wordWithMaxVowels = string.Empty;
        foreach (var article in news)
        {
            var newsValue = GetNewsValue(article, fieldName);
            var words = newsValue.Split(delimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var maxNumOfVowels = 0;
            foreach (var word in words)
            {
                var numOfVowels = word.Count(symbol => vowels.Contains(symbol));

                if (numOfVowels > maxNumOfVowels)
                {
                    maxNumOfVowels = numOfVowels;
                    wordWithMaxVowels = word;
                }
            }
            
            newsContentWithMaxVowelsWords.Add(newsValue, wordWithMaxVowels);
        }

        return newsContentWithMaxVowelsWords;
    }
}