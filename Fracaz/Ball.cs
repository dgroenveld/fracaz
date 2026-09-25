namespace Fracaz;

public class Ball
{
    private float cXpos;
    private float cYpos;
    private float cXvel;
    private float cYvel;
    private float cYstart;
    private float cYtilt;
    private float cYtiltvel;
    private float cYshadow;
    private float cElastic;
    private int cShape;
    private int cColor;
    private bool cEnabled;

    public float Xpos
    {
        get { return cXpos; }
        set { cXpos = value; }
    }

    public float Ypos
    {
        get { return cYpos; }
        set { cYpos = value; }
    }

    public float Xvel
    {
        get { return cXvel; }
        set { cXvel = value; }
    }

    public float Yvel
    {
        get { return cYvel; }
        set { cYvel = value; }
    }

    public float Ystart
    {
        get { return cYstart; }
        set { cYstart = value; }
    }

    public float Ytilt
    {
        get { return cYtilt; }
        set { cYtilt = value; }
    }

    public float Ytiltvel
    {
        get { return cYtiltvel; }
        set { cYtiltvel = value; }
    }

    public float Yshadow
    {
        get { return cYshadow; }
        set { cYshadow = value; }
    }

    public float Elastic
    {
        get { return cElastic; }
        set { cElastic = value; }
    }

    public int Shape
    {
        get { return cShape; }
        set { cShape = value; }
    }

    public int Color
    {
        get { return cColor; }
        set { cColor = value; }
    }

    public bool Enabled
    {
        get { return cEnabled; }
        set { cEnabled = value; }
    }
}
