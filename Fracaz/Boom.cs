using Fracaz.Helpers;
using static Fracaz.Declarations;

namespace Fracaz;

public static class Boom
{
    // This form contains code from Boom! Particle Explosion Simulation.

    // The balls array contains the vital statistics of each ball.
    static Ball[] Balls = new Ball[BALLMAX + ONE_ARRAY_FIX];

    // Water variables dimensions.
    public static int[] WaterColors = new int[6 + ONE_ARRAY_FIX];
    static Wave[] Waves = new Wave[WAVEMAX + ONE_ARRAY_FIX];


    public static void PrepareBallBuffer()
    {
        // This sub simply copies the MapBuffer background picture into the PicBuffer.

        using (var gdiOperations = new GDIOperations(Land.PicBuffer, Land.MapBuffer))
        {
            gdiOperations.BitBlt(0, 0, Wide, Tall, 0, 0, SRCCOPY);
        }
    }

    private static void DrawBall(int i, GDIOperations gdiOperations, int Shape, int Color, int x, int y)
    {
        // Now we draw the new ball.
        if (Balls[i].Shape < 5)
        {
            gdiOperations.BitBlt(x, y, 5, 5, (Shape - 1) * 7, 91, SRCAND);
            gdiOperations.BitBlt(x, y, 5, 5, (Shape - 1) * 7, (Color * 7), SRCINVERT);
        }
        else
        {
            gdiOperations.BitBlt(x - 2, y, 7, 7, (Shape - 1) * 7, 91, SRCAND);
            gdiOperations.BitBlt(x - 2, y, 7, 7, (Shape - 1) * 7, (Color * 7), SRCINVERT);
        }
    }

    private static void DrawShadow(int i, GDIOperations gdiOperations, int Shape, int x, float Newshad)
    {
        // Draw the shadow so that the ball will overlap it.
        if (Balls[i].Shape < 5)
        {
            gdiOperations.BitBlt(x, (int)Newshad, 5, 3, (Shape - 1) * 7, 100, SRCAND);
            gdiOperations.BitBlt(x, (int)Newshad, 5, 3, (Shape - 1) * 7, 97, SRCINVERT);
        }
        else
        {
            gdiOperations.BitBlt(x - 2, (int)Newshad + 5, 7, 3, (Shape - 1) * 7, 101, SRCAND);
            gdiOperations.BitBlt(x - 2, (int)Newshad + 5, 7, 3, (Shape - 1) * 7, 98, SRCINVERT);
        }
    }

    public static void BallTimerSub()
    {
        // This is where we modify each ball's position based on
        // the explosion's properties and global settings.

        // Step 1:  Copy the blank background over.
        PrepareBallBuffer();

        // Step 2:  Ball physics.
        for (var i = 1; i <= BALLMAX; i++)
        {
            // Get out of this loop if there are no balls -- the most we'll have
            // is a single selection arrow.
            if (i > 2 && CFGExplosions == 0)
            {
                break;
            }
            // Only operate on balls that are still bouncing.

            if (Balls[i].Enabled == true)
            {
                // Get the class properties so we can work locally.  Now the values
                // in the class are the 'old' values, used for erasing balls
                // and for shadow calculations.
                var Xpos = Balls[i].Xpos;
                var Ypos = Balls[i].Ypos;
                var Yvel = Balls[i].Yvel;
                var Ytilt = Balls[i].Ytilt;
                var Ytiltvel = Balls[i].Ytiltvel;
                var Ystartloc = Balls[i].Ystart;
                var Absorb = Balls[i].Elastic;
                // Apply gravity to the vertical velocity.
                var Yshadow = 0;

                Yvel = Yvel + GRAVITY;
                // Adjust the tilt of the ball. This is used to skew the pattern for
                // a 3-d type of effect.  A z-axis modifier, if you will.
                Ytilt = Ytilt + Ytiltvel;
                // Now check if we can kill this ball because it's out of bounds.
                // Note that the -5 on the Xpos check is because that is the width
                // of a single ball!  Just a fudge since they were hanging over to the right.

                if (Xpos < 0 || Xpos > Wide - 5 || Balls[i].Yshadow < 0 || Ypos + Ytilt + Ytiltvel > Tall)
                {
                    Balls[i].Enabled = false;
                }
                else
                {
                    // Move ball.
                    Xpos = Xpos + Balls[i].Xvel;
                    Ypos = Ypos + Yvel;

                    // Calculate the shadow position.
                    Yshadow = (int)(Balls[i].Ypos + Ytilt) + (int)(Ystartloc - Balls[i].Ypos) + 2;

                    // If we went past our starting point, we need to rebound.
                    if (Ypos > Ystartloc)
                    {
                        // The ground absorbs some velocity and reverses the ball's direction.
                        Yvel = Absorb * (-Yvel);
                        Ypos = Ystartloc;
                        // If the ball has slowed down enough, or if it has hit water,
                        // we will stop it altogether.
                        if (Math.Abs(Yvel) < 0.5 * GRAVITY)
                        {
                            // Take this ball out of service and free up a slot for a new one.
                            Balls[i].Enabled = false;
                        }

                        if (MyMap!.Grid(((int)(Xpos - 2) / 8) + 1, ((int)(Yshadow - 2) / 8) + 1) > TILEVAL_COASTLINE)
                        {
                            // Take this ball out of service and free up a slot for a new one...
                            Balls[i].Enabled = false;
                            // ...and make a splash since we hit water!
                            MakeSplash((int)Xpos, (int)Yshadow);
                        }
                    }
                }

                // If this ball didn't die, we need to get its shadow position
                // and copy its stats over to the class properties.

                if (Balls[i].Enabled == true)
                {
                    // Update the values of all the parameters in the class.  These will
                    // be the 'old' values on the next scan!
                    Balls[i].Yshadow = Yshadow;
                    Balls[i].Xpos = Xpos;
                    Balls[i].Ypos = Ypos;
                    Balls[i].Yvel = Yvel;
                    Balls[i].Ytilt = Ytilt;
                    Balls[i].Ytiltvel = Ytiltvel;
                    Balls[i].Ystart = Ystartloc;
                }
            }
        }

        // Now update the video buffer.  Note that balls are only drawn to PicBuffer.

        // Step 3:  Draw all the shadows FIRST so that they don't overlap any balls.
        using (var gdiOperations = new GDIOperations(Land.PicBuffer, Land.BallPic))
        {

            for (var i = 1; i <= BALLMAX; i++)
            {
                // Leave if we're only doing selection arrows.
                if (i > 2 && CFGExplosions == 0)
                {
                    break;
                }

                if (Balls[i].Enabled == true)
                {
                    if (Balls[i].Shape < 5 || Balls[i].Shape == 5 && Balls[i].Yvel > 0 || Balls[i].Shape == 6 && Balls[i].Yvel < 0 || Balls[i].Shape > 6)
                    {
                        DrawShadow(i, gdiOperations, Balls[i].Shape, (int)Balls[i].Xpos, Balls[i].Yshadow);
                    }
                }
            }

            //Step 4:  Draw all the balls.
            for (var i = 1; i <= BALLMAX; i++)
            {
                // Leave if we're only doing selection arrows.
                if (i > 2 && CFGExplosions == 0)
                {
                    break;
                }

                if (Balls[i].Enabled == true)
                {
                    if (Balls[i].Shape < 5 || Balls[i].Shape == 5 && Balls[i].Yvel > 0 || Balls[i].Shape == 6 && Balls[i].Yvel < 0 || Balls[i].Shape > 6)
                    {
                        DrawBall(i, gdiOperations, Balls[i].Shape, Balls[i].Color, (int)Balls[i].Xpos, (int)(Balls[i].Ypos + Balls[i].Ytilt));
                    }
                }
            }
        }

        // Leave if we don't want waves.
        if (CFGWaves > 0)
        {
            // Animate the water.
            AnimateWater();
        }
    }


    private static void AnimateWater()
    {

        using (var gdiOperations = new GDIOperations(Land.PicBuffer, Land.WaterPic))
        {
            for (var i = 1; i <= WAVEMAX; i++)
            {
                // Let's see if we can start a random new wave.
                if (Waves[i].Enabled == false && Random.Shared.NextDouble() < 0.02)
                {
                    var x = Random.Shared.Next(MyMap!.Xsize);
                    var y = Random.Shared.Next(MyMap!.Ysize);

                    if (MyMap!.Grid(x, y) > TILEVAL_COASTLINE)
                    {
                        // Found a slot with water!  Make a wave here.
                        Waves[i].Enabled = true;
                        Waves[i].Frame = 0;
                        Waves[i].Count = 0;
                        Waves[i].Shape = Random.Shared.Next(8);
                        Waves[i].Speed = Random.Shared.Next(1, 5);
                        Waves[i].Xpos = (x * 8) + Random.Shared.Next(4);
                        Waves[i].Ypos = (y * 8) + Random.Shared.Next(8);
                    }

                }
                else if (Waves[i].Enabled == true)
                {
                    // Animate this one!
                    Waves[i].Count++;

                    if (Waves[i].Count >= Waves[i].Speed)
                    {
                        // We advance the frame.
                        Waves[i].Count = Waves[i].Count - Waves[i].Speed;
                        Waves[i].Frame++;
                    }

                    // Now draw the wave.
                    if (Waves[i].Frame < 6)
                    {
                        gdiOperations.BitBlt(Waves[i].Xpos, Waves[i].Ypos, 5, 1, (Waves[i].Frame - 1) * 5, (Waves[i].Shape * 2) + 1, SRCAND);
                        gdiOperations.BitBlt(Waves[i].Xpos, Waves[i].Ypos, 5, 1, (Waves[i].Frame - 1) * 5, (Waves[i].Shape * 2), SRCINVERT);
                    }
                    else
                    {
                        // This wave is done.
                        Waves[i].Enabled = false;
                    }
                }
            }
        }
    }

    private static void MakeSplash(int xpos, int yshadow)
    {
        // This function is called whenever a ball hits water.

        if (CFGWaves == 0) return;

        for (var j = 1; j <= WAVEMAX; j++)
        {
            if (Waves[j].Enabled == false)
            {
                // 'We'll put the splash at j.
                Waves[j].Enabled = true;
                Waves[j].Frame = 0;
                Waves[j].Count = 0;
                Waves[j].Speed = Random.Shared.Next(1, 5);
                Waves[j].Shape = Random.Shared.Next(2) + 8;
                Waves[j].Xpos = xpos;
                Waves[j].Ypos = yshadow;
                break;
            }
        }
    }

    public static void ClearAllBalls()
    {

        // This sub cleans out the whole ball array.
        for (int i = 1; i <= BALLMAX; i++)
        {
            Balls[i] = new Ball();
            Balls[i].Enabled = false;
        }
    }

    public static void ClearAllWaves()
    {

        // Clean out the wave array.
        for (int i = 1; i <= WAVEMAX; i++)
        {
            Waves[i] = new Wave();
            Waves[i].Enabled = false;
        }
    }

    public static void BuildBalls(int StartNum, int Xstart, int Ystart, int Intensity, int Spread, int AbsorbPct, int Size, int Color)
    {
        // Leave unless we know we're making a selection arrow.
        if (CFGExplosions == 0 && (Size < 5 || Size > 6)) return;

        // Do this for each new ball.
        for (var j = 1; j <= StartNum; j++)
        {

            // Find an empty ball slot and fill it up with info.
            for (var i = 1; i <= BALLMAX; i++)
            {

                if (Balls[i].Enabled == false)
                {
                    // Slot i is free.  Let's populate it.
                    Balls[i] = new Ball();
                    Balls[i].Enabled = true;
                    Balls[i].Xpos = Xstart;
                    Balls[i].Ypos = Ystart;
                    Balls[i].Xvel = (float)((Random.Shared.NextDouble() * (Spread / 5)) - ((Spread / 5) / 2));

                    if (Intensity == -1) // Used for Computer turns.
                    {
                        Balls[i].Yvel = -11.37f;
                    }
                    else
                    {
                        Balls[i].Yvel = -(float)((Random.Shared.NextDouble() * Intensity / 3) + (Intensity / 20));
                    }

                    Balls[i].Ystart = Ystart;
                    Balls[i].Ytilt = 0;
                    Balls[i].Ytiltvel = (float)(Random.Shared.NextDouble() * (Spread / 5)) - ((Spread / 5) / 2);
                    Balls[i].Yshadow = Ystart + 2;

                    switch (Size)
                    {
                        case 5:
                            // Used for Computer turns.
                            Balls[i].Shape = 5;
                            break;
                        case 6:
                            // Used for Computer turns.
                            Balls[i].Shape = 6;
                            break;
                        case 7:
                            // Used for bonus twinkles.
                            Balls[i].Shape = Random.Shared.Next(4) + 7;
                            break;
                        case 9:
                            // Used for small bonus twinkles.
                            Balls[i].Shape = Random.Shared.Next(2) + 9;
                            break;
                        default:
                            Balls[i].Shape = Random.Shared.Next(Size) + 1;
                            break;
                    }

                    Balls[i].Color = Color;
                    Balls[i].Elastic = AbsorbPct / 100;
                    // Now fudge out of the loop and initialize the next ball.
                    break;
                }
            }

        }
    }

    public static bool NoBalls()
    {
        // This function returns a true if no balls are active,
        // and a false if even one is bouncing.

        for (var i = 1; i <= BALLMAX; i++)
        {
            if (Balls[i].Enabled == true) return false;
        }

        return true;
    }
}