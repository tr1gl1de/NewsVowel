using System.CommandLine;
using static NewsVowelTask.NewsParseExtensions;

namespace NewsVowelTask;

static class Program
{
    static async Task<int> Main(string[] args)
    {
        var themeNewsOption = new Option<string>(
            name: "--theme",
            description: "Тема новости для парсинга",
            getDefaultValue: () => "космос");

        var filedNewsOption = new Option<string>(
            name: "--field",
            description: "Поле новости для чтения \"author, title, description, content\"",
            getDefaultValue: () => "title");

        var newsApiKeyOption = new Option<string>(
            name: "--key",
            description: "Ключ от API newsapi");

        var pageSizeOption = new Option<int>(
            name: "--pageSize",
            description: "количество элементов на странице для парсинга",
            getDefaultValue: () => 1);
        
        var pageNumberOption = new Option<int>(
            name: "--pageNum",
            description: "Номер страницы для парсинга",
            getDefaultValue: () => 1);
        
        var rootCommand =
            new RootCommand("Программа для парсинга и поиска в новостях слова с наибольшим количеством гласных");

        var parseNewsCommand = new Command("parse", "Парсинг и поиск слова")
        {
            themeNewsOption,
            filedNewsOption,
            newsApiKeyOption,
            pageSizeOption,
            pageNumberOption
        };
        rootCommand.AddCommand(parseNewsCommand);
        
        parseNewsCommand.SetHandler(async (key, theme, fieldName, pageSize, pageNum) =>
        {
            var news = await ParseNews(key, theme, pageNum, pageSize);
            var result = news.GetVowelsFromNews(fieldName);
            PrintResult(result);
        }, newsApiKeyOption, themeNewsOption, filedNewsOption, pageSizeOption, pageNumberOption);
        
        return await rootCommand.InvokeAsync(args);
    }

    private static void PrintResult<T, T2>(IReadOnlyDictionary<T, T2> res)
    {
        foreach (var keyValuePair in res)
        {
            Console.WriteLine($"Key:[{keyValuePair.Key}] Value:[{keyValuePair.Value}]");
        }
    }
}