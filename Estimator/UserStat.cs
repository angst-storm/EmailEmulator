namespace Estimator;

public class UserStat
{
    private readonly int[] clicksOnTheme = new int[50];

    public (int, int, int) ThemesInDb
    {
        get;
    }

    public (int, int, int) TopThemes
    {
        get
        {
            var result = clicksOnTheme
                .Select((n, i) => (i, n))
                .OrderByDescending(n => n.n)
                .Take(3)
                .OrderBy(n => n.i)
                .Select(n => n.i + 1)
                .ToArray();
            return (result[0], result[1], result[2]);
        }
    }

    public UserStat(int[] themes)
    {
        themes = themes.OrderBy(n => n).ToArray();
        ThemesInDb = (themes[0], themes[1], themes[2]);
    }

    public void AddThemes(int[] themes)
    {
        foreach (var theme in themes)
            clicksOnTheme[theme - 1]++;
    }
}