using UnityEngine;

public sealed class IPath {
    private float _scale;
    private float _radius;
    private float _radians;

    public IPath(float scale, float radius, float radians) {
        this._scale = scale;
        this._radius = radius;
        this._radians = radians;
    }
    
    public void Advance(float deltaTime) {
        this._radians += deltaTime / (this._scale * this._radius);
        this._radians %= Mathf.PI * 2;
        this._radians += deltaTime / (this._scale * this._radius);
        this._radians %= Mathf.PI * 2;
    }

    public float Sample() {
        float x = this._radius * Mathf.Cos(this._radians);
        float y = this._radius * Mathf.Sin(this._radians);

        return Mathf.PerlinNoise(x, y);
    }
}
