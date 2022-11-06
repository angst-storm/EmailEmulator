using System.Text.Json;
using Shared;

namespace Estimator;

public static class Program
{
    private const string ConnString =
        "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=tglc1996;";

    private const string Server = "localhost:9092";
    private const string Group = "estimator";
    private const string ClicksTopic = "clicks";
    private const string CommandsTopic = "commands";
    private static readonly RedPanda RedPanda = new(Server, Group);

    public static Dictionary<int, UserStat> UserStats = new();
    public static int SentCount { get; private set; }

    public static void Main(string[] args)
    {
        RedPanda.Subscribe(ClicksTopic);

        InitializeUserStats(false);

        Task.Run(AnalyzeProcess);

        InitializeWepApp(args);
    }

    private static void InitializeUserStats(bool newUsers)
    {
        var db = new DataBase(ConnString);
        if (newUsers)
        {
            db.InitializeTables();
            var numbers = Enumerable.Range(1, 50).ToArray();
            var random = new Random();
            foreach (var number in numbers)
            {
                var themes = numbers
                    .Select(n => (n, random.Next()))
                    .OrderBy(ni => ni.Item2)
                    .Select(n => n.n)
                    .Take(3)
                    .ToArray();
                db.AddUser($"User{number}", themes);
            }
        }

        var users = db.GetUsers();
        UserStats = users.ToDictionary(u => int.Parse(u.Item1[4..]), u => new UserStat(u.Item2));
    }

    private static void InitializeWepApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();

        var app = builder.Build();

        app.MapRazorPages();

        app.Run("https://localhost:7226");
    }

    public static void SendMails(int count)
    {
        RedPanda.Produce(CommandsTopic, $"send {count}");
        SentCount += count;
    }

    private static void AnalyzeProcess()
    {
        while (true)
        {
            var cr = RedPanda.Consume();
            var click = JsonSerializer.Deserialize<Click>(cr.Message.Value);
            if (click is null)
                throw new Exception();
            UserStats[int.Parse(click.username[4..])].AddThemes(click.mailThemes);
        }
    }
}