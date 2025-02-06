//This code provided by https://discussions.unity.com/t/cinemachine-collider-pull-camera-forward-strategy/938609/5

using Cinemachine;
using UnityEngine;

[ExecuteAlways]
public class RestoreReferenceLookAt : CinemachineExtension
{
    [Tooltip("When to restore the ReferenceLookAt (for CinemachineCollider, use Body)")]
    public CinemachineCore.Stage Stage = CinemachineCore.Stage.Body;

    Vector3 m_SavedReferenceLookAt;

    public override void PrePipelineMutateCameraStateCallback(
        CinemachineVirtualCameraBase vcam, ref CameraState curState, float deltaTime)
    {
        // Stash the LookAt target
        m_SavedReferenceLookAt = curState.ReferenceLookAt;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == Stage)
            state.ReferenceLookAt = m_SavedReferenceLookAt;
    }
}