using static Fracaz.Declarations;

namespace Fracaz;

internal static class SoundFX
{
    public static void PlayWav(string strPath, int sndVal)
    {
        // Don't play a sound if sfx are disabled.
        if (CFGSound == 2)
            return;

        sndPlaySound(SFXpath + strPath, sndVal);
    }

    public static void SNDTroopsIn(int PlayrNum)
    {
        PlayWav("dn" + PlayrNum + ".wav", SND_GAME_FLAGS);
    }

    public static void SNDTroopsOut(int PlayrNum)
    {
        PlayWav("up" + PlayrNum + ".wav", SND_GAME_FLAGS);
    }

    public static void SNDBuildAPort()
    {
        PlayWav("port" + Random.Shared.Next(1, 4) + ".wav", SND_GAME_FLAGS);
    }

    public static void SNDPlayWarCadence()
    {
        if (Random.Shared.NextDouble() < 0.3)
        {
            PlayWav("timpani1.wav", SND_GAME_FLAGS);
        }
        else
        {
            PlayWav("timpani2.wav", SND_GAME_FLAGS);
        }
    }

    public static void SNDPlayFanfare(int FanfareNum)
    {
        PlayWav("fanfare" + FanfareNum.ToString() + ".wav", SND_GAME_FLAGS);
    }

    public static void SNDSmallExplosion()
    {
        PlayWav("smexp" + Random.Shared.Next(1, 4) + ".wav", SND_GAME_FLAGS);
    }

    public static void SNDMediumExplosion()
    {
        PlayWav("mdexp" + Random.Shared.Next(1, 4) + ".wav", SND_GAME_FLAGS);
    }

    public static void SNDLargeExplosion()
    {
        PlayWav("lgexp" + Random.Shared.Next(1, 4) + ".wav", SND_GAME_FLAGS);
    }

    public static void SNDWhistle()
    {
        PlayWav("ladeda" + Random.Shared.Next(1, 4) + ".wav", SND_GAME_FLAGS);
    }

    public static void SNDApplause()
    {
        PlayWav("yay" + Random.Shared.Next(1, 3) + ".wav", SND_GAME_FLAGS);
    }

    public static void SNDSplishSplash()
    {
        PlayWav("splash" + Random.Shared.Next(1, 4) + ".wav", SND_GAME_FLAGS);
    }

    public static void SNDBooBoo()
    {
        PlayWav("booboo.wav", SND_GAME_FLAGS);
    }

    public static void SNDBonusTwinkles()
    {
        PlayWav("bonus" + Random.Shared.Next(1, 4) + ".wav", SND_GAME_FLAGS);
    }
}
