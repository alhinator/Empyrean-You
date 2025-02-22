using System;

/// <summary>
/// A registry of guns containing translation keys and other data
/// </summary>
public sealed class Frames
{
    private Frames() { }
    public static readonly FrameEntry[] all = {
        new("bast.name", "bast.splash", typeof(Bast)),
        new("itzi.name", "itzi.splash", typeof(Itzi))
    };

    public static readonly FrameIndex BAST = new(0);
    public static readonly FrameIndex ITZI = new(1);
}

/// <summary>
/// A record containing all data specific to a certain gun that needs to be
/// known before an instance of the gun itself is to be rendered
/// </summary>
public class FrameEntry
{
    public readonly string NameTranslationKey;
    public readonly string SplashTranslationKey;

    public readonly Type ScriptName;

    public FrameEntry(string nameTranslationKey, string splashTranslationKey, Type scriptName)
    {
        this.NameTranslationKey = nameTranslationKey;
        this.SplashTranslationKey = splashTranslationKey;
        this.ScriptName = scriptName;
    }
}

public struct FrameIndex
{
    public readonly int index;

    public FrameIndex(int index)
    {
        this.index = index;
    }

    public readonly string NameTranslationKey => ((FrameEntry)this).NameTranslationKey;
    public readonly string SplashTranslationKey => ((FrameEntry)this).SplashTranslationKey;
    public readonly Type Script => ((FrameEntry)this).ScriptName;

    public static FrameIndex operator ++(FrameIndex now)
    {
        return new((now.index + 1) % Guns.all.Length);
    }

    public static FrameIndex operator --(FrameIndex now)
    {
        int nextIndex = now.index - 1;
        if (nextIndex < 0) nextIndex = Guns.all.Length - 1;
        return new(nextIndex);
    }

    public static implicit operator FrameEntry(FrameIndex idx)
    {
        return Frames.all[idx.index];
    }
}
