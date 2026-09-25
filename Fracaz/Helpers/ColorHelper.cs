namespace Fracaz.Helpers;

internal static class ColorHelper
{
    public static int RGB(int red, int green, int blue)
    {
        return (255 << 24) | (red << 16) | (green << 8) | blue;
    }
}
