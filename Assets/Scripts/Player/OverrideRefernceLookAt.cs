//This code provided by https://discussions.unity.com/t/cinemachine-collider-pull-camera-forward-strategy/938609/5
using Cinemachine;
using UnityEngine;

[ExecuteAlways]
public class OverrideReferenceLookAt : CinemachineExtension
{
    [Tooltip("When to apply the Override (for CinemachineCollider, use Body)")]
    public CinemachineCore.Stage Stage = CinemachineCore.Stage.Body;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        // Use the Follow target as the LookAt target, but only for the specified stage
        var follow = vcam.Follow;
        if (follow != null && stage == Stage)
            state.ReferenceLookAt = follow.position;
    }
}