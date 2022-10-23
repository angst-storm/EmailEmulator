namespace Analyzer;

public class UserStat
{
    private readonly int[] clicksOnTheme = new int[50];
    private (int, int, int) themesInDb;

    public UserStat(int[] themes)
    {
        themesInDb = (themes[0], themes[1], themes[2]);
    }

    public void AddThemes(int[] themes)
    {
        foreach (var theme in themes)
            clicksOnTheme[theme - 1]++;
    }

    public override string ToString()
    {
        var result = clicksOnTheme
            .Select((n, i) => (i, n))
            .OrderByDescending(n => n.n)
            .ToArray();
        return $"calculate: [{result[0].i + 1}, {result[1].i + 1}, {result[2].i + 1}]\n" +
               $"on db: [{themesInDb.Item1}, {themesInDb.Item2}, {themesInDb.Item3}]";
    }
}