namespace ApplicationCore.ValueObjects.Nip;

public static class NipParser
{
    public static bool TryParse(string input, out Nip? nip)
    {
        nip = null;
        try { nip = new Nip(input); return true; }
        catch (ArgumentException) { return false; }
    }
}
