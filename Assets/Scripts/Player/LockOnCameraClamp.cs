using UnityEngine;
using Cinemachine;  
//This code taken from https://discussions.unity.com/t/when-too-close-to-lookat-target-using-composer-the-camera-will-spin-indefinitely/905366/5
public class LockOnCameraClamp : CinemachineExtension
{
    public float minAngle = -20;
    public float maxAngle = 20;
  
    /// <summary>
    /// When to apply the offset
    /// </summary>
    [Tooltip("When to apply the offset")]
    public CinemachineCore.Stage m_ApplyAfter = CinemachineCore.Stage.Aim;
  

    /// <summary>
    /// Applies the specified offset to the camera state
    /// </summary>
    /// <param name="vcam">The virtual camera being processed</param>
    /// <param name="stage">The current pipeline stage</param>
    /// <param name="state">The current virtual camera state</param>
    /// <param name="deltaTime">The current applicable deltaTime</param>
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == m_ApplyAfter)
        {
            var newX = ClampAngle(state.RawOrientation.eulerAngles.x, minAngle, maxAngle);
            var newEuler = new Vector3(newX, state.RawOrientation.eulerAngles.y, state.RawOrientation.eulerAngles.z);
            state.RawOrientation.eulerAngles = newEuler;
        }
    }
  
    private static float ClampAngle(float current, float min, float max)
    {
        float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
        float hdtAngle = dtAngle * 0.5f;
        float midAngle = min + hdtAngle;
        float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
        if (offset > 0)
        {
            current = Mathf.MoveTowardsAngle(current, midAngle, offset);
        }

        return current;
    }
}