using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

namespace ChiliGames.VROffice
{
    public class HandAnimator : MonoBehaviour
    {
        public float speed = 5.0f;
        public XRController controller = null;
        public ActionBasedController controllerAction = null;
        [SerializeField] bool leftHand;
        PhotonView pv;
        bool isActionBasedController;

        private Animator animator = null;

        private readonly List<Finger> gripfingers = new List<Finger>()
        {
            new Finger(FingerType.Middle),
            new Finger(FingerType.Ring),
            new Finger(FingerType.Pinky)
        };

        private readonly List<Finger> pointFingers = new List<Finger>
        {
            new Finger(FingerType.Index),
            new Finger(FingerType.Thumb)
        };

        private void Start()
        {
            pv = GetComponent<PhotonView>();
            animator = GetComponent<Animator>();
            if (!pv.IsMine) return;
            if (leftHand)
            {
                if(PlatformManager.instance.vrRigParts[1].GetComponentInChildren<XRController>() != null)
                {
                    controller = PlatformManager.instance.vrRigParts[1].GetComponentInChildren<XRController>();
                }
                if (PlatformManager.instance.vrRigParts[1].GetComponentInChildren<ActionBasedController>() != null)
                {
                    controllerAction = PlatformManager.instance.vrRigParts[1].GetComponentInChildren<ActionBasedController>();
                    isActionBasedController = true;
                }

            }
            else
            {
                if (PlatformManager.instance.vrRigParts[2].GetComponentInChildren<XRController>() != null)
                {
                    controller = PlatformManager.instance.vrRigParts[2].GetComponentInChildren<XRController>();
                }
                if (PlatformManager.instance.vrRigParts[2].GetComponentInChildren<ActionBasedController>() != null)
                {
                    controllerAction = PlatformManager.instance.vrRigParts[2].GetComponentInChildren<ActionBasedController>();
                    isActionBasedController = true;
                }
            }
        }

        private void Update()
        {
            if (!pv.IsMine) return;
            // Store input
            CheckGrip();
            CheckPointer();

            // Smooth input values
            SmoothFinger(pointFingers);
            SmoothFinger(gripfingers);

            // Apply smoothed values
            AnimateFinger(pointFingers);
            AnimateFinger(gripfingers);
        }

        private void CheckGrip()
        {
            if (isActionBasedController)
            {
                float gripValue = controllerAction.selectAction.action.ReadValue<float>();
                SetFingerTargets(gripfingers, gripValue);
            }
            else
            {
                if (controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
                    SetFingerTargets(gripfingers, gripValue);
            }
        }

        private void CheckPointer()
        {
            if (isActionBasedController)
            {
                float gripValue = controllerAction.selectAction.action.ReadValue<float>();
                SetFingerTargets(gripfingers, gripValue);
            }
            else
            {
                if (controller.inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
                    SetFingerTargets(gripfingers, gripValue);
            }
        }

        private void SetFingerTargets(List<Finger> fingers, float value)
        {
            foreach (Finger finger in fingers)
                finger.target = value;
        }

        private void SmoothFinger(List<Finger> fingers)
        {
            foreach(Finger finger in fingers)
            {
                float time = speed * Time.unscaledDeltaTime;
                finger.current = Mathf.MoveTowards(finger.current, finger.target, time);
            }
        }

        private void AnimateFinger(List<Finger> fingers)
        {
            foreach (Finger finger in fingers)
                AnimateFinger(finger.type.ToString(), finger.current);
        }

        private void AnimateFinger(string finger, float blend)
        {
            animator.SetFloat(finger, blend);
        }
    }
}
