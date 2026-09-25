namespace Fracaz;

public class Wave
{
    private int cXpos;
    private int cYpos;
    private float cSpeed;
    private float cCount;
    private int cFrame;
    private int cShape;
    private bool cEnabled;

    public int Xpos
    {
        get { return cXpos; }
        set { cXpos = value; }
    }

    public int Ypos
    {
        get { return cYpos; }
        set { cYpos = value; }
    }

    public float Speed
    {
        get { return cSpeed; }
        set { cSpeed = value; }
    }

    public float Count
    {
        get { return cCount; }
        set { cCount = value; }
    }

    public int Frame
    {
        get { return cFrame; }
        set { cFrame = value; }
    }

    public int Shape
    {
        get { return cShape; }
        set { cShape = value; }
    }

    public bool Enabled
    {
        get { return cEnabled; }
        set { cEnabled = value; }
    }
}
