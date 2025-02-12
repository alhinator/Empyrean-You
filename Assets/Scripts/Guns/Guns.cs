using System;

/// <summary>
/// A registry of guns containing translation keys and other data
/// </summary>
public sealed class Guns {
    private Guns() {}
    public static readonly GunEntry[] all = {
        new("artemis.name", "artemis.splash"),
        new("guanyin.name", "guanyin.splash")
    };

    public static readonly GunIndex ARTEMIS = new(0);
    public static readonly GunIndex GUANYIN = new(1);
}

/// <summary>
/// A record containing all data specific to a certain gun that needs to be
/// known before an instance of the gun itself is to be rendered
/// </summary>
public class GunEntry {
    public readonly string NameTranslationKey;
    public readonly string SplashTranslationKey;

    public GunEntry(string nameTranslationKey, string splashTranslationKey) {
        this.NameTranslationKey = nameTranslationKey;
        this.SplashTranslationKey = splashTranslationKey;
    }
}

public struct GunIndex {
    public readonly int index;
    
    public GunIndex(int index) {
        this.index = index;
    }

    public readonly string NameTranslationKey => ((GunEntry) this).NameTranslationKey;
    public readonly string SplashTranslationKey => ((GunEntry) this).SplashTranslationKey;

    public static GunIndex operator ++(GunIndex now) {
        return new((now.index + 1) % Guns.all.Length);
    }

    public static GunIndex operator --(GunIndex now) {
        int nextIndex = now.index - 1;
        if(nextIndex < 0) nextIndex = Guns.all.Length - 1;
        return new(nextIndex);
    }

    public static implicit operator GunEntry(GunIndex idx) {
        return Guns.all[idx.index];
    }
}
