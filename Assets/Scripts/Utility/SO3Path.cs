using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate a smooth random walk in SO(3)
/// 
/// Possible solution: Double axis-angle
/// Step 1: we can generate 3 closed walks in [0, 1], called ⟨u₁, u₂, u₃⟩ via 3 calls to 2d perlin noise
/// Step 2: we can choose a latitude θ₁ = acos(2u₁ - 1)
/// Step 3: we can choose a longitude φ₂ = 2πu₂
/// Step 4: we can choose a rotation ψ₃ = 2πu₃
/// Step 5: repeat the above steps again for θ₂, φ₂, ψ₂
/// Step 6: generate a quaternion representing each axis-angle h₁ and h₂
/// Step 7: find the quaternion q such that qh₁ = h₂
/// Step 8: return q
///
/// Caveats:
/// - expensive?
/// - "hides" the poles issue by just taking a different random pole
/// </summary>
public sealed class SO3Path {
    private O3Path firstSphere;
    private IPath firstAngle;
    private O3Path secondSphere;
    private IPath secondAngle;
    
    public SO3Path(
        float scale,
        float radius1,
        float radius2,
        float radius3,
        float radius4,
        float radius5,
        float radius6,
        float radians1,
        float radians2,
        float radians3,
        float radians4,
        float radians5,
        float radians6
    ) {
        this.firstSphere = new O3Path(scale, radius1, radius2, radians1, radians2);
        this.firstAngle = new IPath(scale, radius3, radians3);
        this.secondSphere = new O3Path(scale, radius4, radius5, radians4, radius5);
        this.secondAngle = new IPath(scale, radius6, radians6);
    }

    public void Advance(float deltaTime) {
        this.firstSphere.Advance(deltaTime);
        this.firstAngle.Advance(deltaTime);
        this.secondSphere.Advance(deltaTime);
        this.secondAngle.Advance(deltaTime);
    }

    public Quaternion Sample() {
        Vector3 axis1 = this.firstSphere.Sample();
        float angle1 = 360f * this.firstAngle.Sample();
        Quaternion q1 = Quaternion.AngleAxis(angle1, axis1);
        
        Vector3 axis2 = this.secondSphere.Sample();
        float angle2 = 360f * this.secondAngle.Sample();
        Quaternion q2 = Quaternion.AngleAxis(angle2, axis2);

        return Quaternion.Inverse(q1) * q2;
    }
}
