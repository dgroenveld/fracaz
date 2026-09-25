using System.Runtime.InteropServices;

namespace Fracaz.Helpers;

internal class GDIOperations : IDisposable
{
    private Graphics? destinationGraphics;
    private nint destinationHdc;
    private nint memDC = nint.Zero;
    private nint hBitmap = nint.Zero;

    [DllImport("gdi32.dll")]
    public static extern long SetPixel(nint hdc, int X, int Y, int crColor);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(nint hObject);

    [DllImport("gdi32.dll")]
    public static extern nint CreateCompatibleDC(nint hdc);

    [DllImport("gdi32.dll")]
    public static extern nint SelectObject(nint hdc, nint hgdiobj);

    [DllImport("gdi32.dll")]
    public static extern bool BitBlt(nint hdcDest, int nXDest, int nYDest,
                   int nWidth, int nHeight, nint hdcSrc,
                              int nXSrc, int nYSrc, int dwRop);


    public GDIOperations(Bitmap destinationBitmap)
    {
        destinationGraphics = Graphics.FromImage(destinationBitmap);
        destinationHdc = destinationGraphics.GetHdc();
    }

    public GDIOperations(Control formControl, Bitmap sourceBitmap)
    {
        destinationGraphics = formControl.CreateGraphics();
        destinationHdc = destinationGraphics.GetHdc();

        memDC = CreateCompatibleDC(destinationHdc);
        hBitmap = sourceBitmap.GetHbitmap();

        SelectObject(memDC, hBitmap);
    }

    private bool _graphicsProvided = false;
    public GDIOperations(Graphics graphics, Bitmap sourceBitmap)
    {
        _graphicsProvided = true;
        destinationGraphics = graphics;
        destinationHdc = destinationGraphics.GetHdc();

        memDC = CreateCompatibleDC(destinationHdc);
        hBitmap = sourceBitmap.GetHbitmap();

        SelectObject(memDC, hBitmap);
    }

    public GDIOperations(Bitmap destinationBitmap, Bitmap bmp) : this(destinationBitmap)
    {
        memDC = CreateCompatibleDC(destinationHdc);
        hBitmap = bmp.GetHbitmap();

        SelectObject(memDC, hBitmap);
    }

    public void Dispose()
    {
        if (hBitmap != nint.Zero)
        {
            DeleteObject(hBitmap);
        }

        if (memDC != nint.Zero)
        {
            DeleteObject(memDC);
        }

        destinationGraphics!.ReleaseHdc(destinationHdc);

        if (!_graphicsProvided)
        {
            destinationGraphics.Dispose();
        }

        hBitmap = nint.Zero;
        memDC = nint.Zero;
    }

    internal void SetPixel(int x, int y, int color)
    {
        SetPixel(destinationHdc, x, y, color);
    }

    internal void BitBlt(int nXDest, int nYDest, int nWidth, int nHeight, int nXSrc, int nYSrc, int dwRop)
    {
        _BitBlt(destinationHdc, nXDest, nYDest, nWidth, nHeight, memDC, nXSrc, nYSrc, dwRop);
    }

    internal void BitBltFromSourceToSource(int nXDest, int nYDest, int nWidth, int nHeight, int nXSrc, int nYSrc, int dwRop)
    {
        _BitBlt(memDC, nXDest, nYDest, nWidth, nHeight, memDC, nXSrc, nYSrc, dwRop);
    }

    private void _BitBlt(nint destinationHdc, int nXDest, int nYDest, int nWidth, int nHeight, nint memDC, int nXSrc, int nYSrc, int dwRop)
    {
        BitBlt(destinationHdc, nXDest, nYDest, nWidth, nHeight, memDC, nXSrc, nYSrc, dwRop);
    }
}