namespace Estimator;

public class UserStat
{
    private readonly int[] clicksOnTheme = new int[50];

    public UserStat(int[] themes)
    {
        ThemesInDb = themes;
    }

    public int this[int theme] => clicksOnTheme[theme - 1];

    public int[] ThemesInDb { get; }

    public int[] TopThemes
    {
        get
        {
            return clicksOnTheme
                .Select((n, i) => (i, n))
                .OrderByDescending(n => n.n)
                .Take(3)
                .Select(n => n.i + 1)
                .ToArray();
        }
    }

    public void AddThemes(int[] themes)
    {
        foreach (var theme in themes)
            clicksOnTheme[theme - 1]++;
    }
}