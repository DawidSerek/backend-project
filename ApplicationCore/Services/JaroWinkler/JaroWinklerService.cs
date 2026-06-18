namespace ApplicationCore.Services.JaroWinkler;

public static class JaroWinklerService
{
    public static double Similarity(string? s1, string? s2)
    {
        if (s1 is null || s2 is null) return 0.0;
        if (s1 == s2) return 1.0;
        if (s1.Length == 0 || s2.Length == 0) return 0.0;

        double jaro = JaroDistance(s1, s2);
        if (jaro < 0.7) return jaro;  // prefix bonus only helps close matches

        // Winkler prefix bonus
        int prefixLen = 0;
        int maxPrefix = Math.Min(4, Math.Min(s1.Length, s2.Length));
        for (int i = 0; i < maxPrefix; i++)
        {
            if (s1[i] == s2[i]) prefixLen++;
            else break;
        }
        return jaro + (prefixLen * 0.1 * (1.0 - jaro));
    }

    private static double JaroDistance(string s1, string s2)
    {
        int len1 = s1.Length, len2 = s2.Length;
        int matchDistance = (Math.Max(len1, len2) / 2) - 1;
        if (matchDistance < 0) matchDistance = 0;

        var s1Matches = new bool[len1];
        var s2Matches = new bool[len2];
        int matches = 0;

        for (int i = 0; i < len1; i++)
        {
            int start = Math.Max(0, i - matchDistance);
            int end = Math.Min(i + matchDistance + 1, len2);

            for (int j = start; j < end; j++)
            {
                if (s2Matches[j]) continue;
                if (s1[i] != s2[j]) continue;
                s1Matches[i] = true;
                s2Matches[j] = true;
                matches++;
                break;
            }
        }

        if (matches == 0) return 0.0;

        // Count transpositions
        int k = 0, transpositions = 0;
        for (int i = 0; i < len1; i++)
        {
            if (!s1Matches[i]) continue;
            while (!s2Matches[k]) k++;
            if (s1[i] != s2[k]) transpositions++;
            k++;
        }

        double m = matches;
        return ((m / len1) + (m / len2) + ((m - (transpositions / 2.0)) / m)) / 3.0;
    }
}
