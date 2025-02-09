using System;
using UnityEngine;

public sealed class O3Path {
    private IPath _latitude;
    private IPath _longitude;

    public O3Path(
        float scale,
        float radius1,
        float radius2,
        float radians1,
        float radians2
    ) {
        this._latitude = new IPath(scale, radius1, radians1);
        this._longitude = new IPath(scale, radius2, radians2);
    }

    public void Advance(float deltaTime) {
        this._latitude.Advance(deltaTime);
        this._longitude.Advance(deltaTime);
    }

    public Vector3 Sample() {
        float u1 = this._latitude.Sample();
        float u2 = this._longitude.Sample();

        float z = (2 * u1) - 1;
        
        float baseRadius = Mathf.Sqrt(1 - (z * z));
        float longitude = 2 * Mathf.PI * u2;
        
        float x = baseRadius * Mathf.Cos(longitude);
        float y = baseRadius * Mathf.Sin(longitude);

        return new Vector3(x, y, z);
    }
}
