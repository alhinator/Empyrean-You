using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This code is from https://gist.github.com/PixelEnvision/b6ff5d1130a5ec657754b6e97d577f28

using Cinemachine;

[AddComponentMenu("")] // Hide in menu
[SaveDuringPlay]
#if UNITY_2018_3_OR_NEWER
[ExecuteAlways]
#else
[ExecuteInEditMode]
#endif
public class CinemachineGroundChecker : CinemachineExtension
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float AdjustmentSpeed = 5;

    RaycastHit hit;
    Vector3 initialOffset;
    Vector3 offset;
    //float offsetY;

    protected override void Awake()
    {
        base.Awake();
        //This line modified to use freeLookCam
        initialOffset = GetComponent<CinemachineFreeLook>().GetRig(1).GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        initialOffset.z = 0;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body && groundLayer.value != 0)
        {
            var camPos = state.CorrectedPosition;
            camPos.y += initialOffset.y;

            //Debug.DrawRay(camPos, Vector3.down * initialOffset);
            //Debug.DrawRay(camPos + (Vector3.down * initialOffset), Vector3.down * initialOffset, Color.red);

            if (Physics.Raycast(camPos, -state.ReferenceUp, out hit, initialOffset.y * 2, groundLayer, QueryTriggerInteraction.Ignore))
            {
                //offsetY = Mathf.Lerp(offsetY, initialOffset.y - (state.CorrectedPosition.y - hit.point.y), deltaTime * AdjustmentSpeed);
                offset = Vector3.Lerp(offset, initialOffset - (state.CorrectedPosition - hit.point), deltaTime * AdjustmentSpeed);
            }
            else
            {
                //offsetY = Mathf.Lerp(offsetY, 0, deltaTime * 0.5f);
                offset = Vector3.Lerp(offset, Vector3.zero, deltaTime * 0.5f);
            }

            //if (offsetY >= 0)
            //    state.PositionCorrection += new Vector3(0, offsetY, 0);

            state.PositionCorrection += offset;
        }
    }
}
