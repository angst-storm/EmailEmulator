namespace Estimator;

public class UserStat
{
    public UserStat(int userId, int[] themes)
    {
        UserId = userId;
        ThemesInDb = themes;
        ClicksOnTheme = new int[50];
    }

    public int UserId { get; }

    public int[] ThemesInDb { get; }

    public int TotalClicksCount { get; private set; }

    public int[] ClicksOnTheme { get; }

    public int this[int theme] => ClicksOnTheme[theme - 1];

    public double Error
    {
        get
        {
            var correctlyGuessed = GetPreferenceByClicksCount().Intersect(ThemesInDb).Count();
            var wronglyGuessed = GetPreferenceByClicksCount().Except(ThemesInDb).Count();
            var error = 1 - ((double)correctlyGuessed / ThemesInDb.Length -
                (double)wronglyGuessed / (50 - ThemesInDb.Length) + 1) / 2;
            return error;
        }
    }

    public void AddClick(int[] themes)
    {
        TotalClicksCount++;
        foreach (var theme in themes)
            ClicksOnTheme[theme - 1]++;
    }

    public int[] GetPreferenceByClicksCount(int count = 5)
    {
        return ClicksOnTheme
            .Select((n, i) => (i, n))
            .OrderByDescending(n => n.n)
            .Take(count)
            .Select(n => n.i + 1)
            .ToArray();
    }

    public int[] GetPreferencesByPartsOfClicks(int minPercent = 12)

    {
        return GetThemesPartsOfClicks()
            .Select((p, i) => (p, i))
            .Where(pi => pi.p > minPercent)
            .Select(pi => pi.i)
            .ToArray();
    }

    public int[] GetThemesPartsOfClicks()
    {
        return ClicksOnTheme
            .Select(clicks =>
            {
                if (TotalClicksCount == 0)
                    return 0;
                return (int)(Math.Round((double)clicks / TotalClicksCount, 2) * 100);
            })
            .ToArray();
    }
}