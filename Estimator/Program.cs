using System.Text.Json;
using Shared;

namespace Estimator;

public static class Program
{
    private const string ConnString =
        "Server=postgres;Port=5432;Database=postgres;User Id=postgres;Password=tglc1996;";

    private const string Server = "redpanda:9092";
    private const string Group = "estimator";
    private const string ClicksTopic = "clicks";
    private const string CommandsTopic = "commands";
    private static readonly RedPanda RedPanda = new(Server, Group);
    public static readonly UserStat[] UserStats = InitializeUserStats(false);
    public static int SentCount { get; private set; }

    public static void Main(string[] args)
    {
        RedPanda.Subscribe(ClicksTopic);

        Task.Run(AnalyzeProcess);

        InitializeWepApp(args);
    }

    private static UserStat[] InitializeUserStats(bool newUsers)
    {
        var db = new DataBase(ConnString);
        if (newUsers)
        {
            db.InitializeTables();
            foreach (var themes in CreateUsersThemes())
                db.AddUser(themes);
        }

        var users = db.GetUsersThemes();
        return users
            .Select((themes, i) => new UserStat(i + 1, themes.ToArray()))
            .ToArray();
    }

    private static IEnumerable<int[]> CreateUsersThemes()
    {
        var numbers = Enumerable.Range(1, 50).ToArray();
        var random = new Random();
        for (var i = 0; i < 50; i++)
            yield return numbers
                .Select(n => (n, random.Next()))
                .OrderBy(ni => ni.Item2)
                .Select(n => n.n)
                .Take(5)
                .ToArray();
    }

    private static void InitializeWepApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();

        var app = builder.Build();

        app.MapRazorPages();

        app.Run("http://net-estimator:7226");
    }

    public static void SendMails(int count)
    {
        RedPanda.Produce(CommandsTopic, $"send {count}");
        SentCount += count;
    }

    private static void AnalyzeProcess()
    {
        for (var i = 0;;i++)
        {
            var cr = RedPanda.Consume();
            Console.WriteLine($"consumed: {cr.Message.Value}");
            var click = JsonSerializer.Deserialize<Click>(cr.Message.Value);
            if (click is null)
                throw new Exception();
            UserStats[click.userId - 1].AddClick(click.mailThemes);
            if (i % 100 == 0)
            {
                var averageError = UserStats.Average(us => us.Error);
                RedPanda.Produce("errors", new Error(i, averageError));
            }
        }
    }
}