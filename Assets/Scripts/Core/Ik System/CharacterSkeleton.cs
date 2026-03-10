using UnityEngine;

public class CharacterSkeleton : MonoBehaviour
{
    public Animator animator;

    #region BonesReferences

    public Transform   Root,
                Hips,
                Spine,
                Chest,
                UpperChest,

                L_Shoulder,
                L_UpperArm,
                L_LowerArm,
                L_Hand,
                L_Thumb_01,
                L_Thumb_02,
                L_Thumb_03,
                L_Index_01,
                L_Index_02,
                L_Index_03,
                L_Middle_01,
                L_Middle_02,
                L_Middle_03,
                L_Ring_01,
                L_Ring_02,
                L_Ring_03,
                L_Pinky_01,
                L_Pinky_02,
                L_Pinky_03,

                R_Shoulder,
                R_UpperArm,
                R_LowerArm,
                R_Hand,
                R_Thumb_01,
                R_Thumb_02,
                R_Thumb_03,
                R_Index_01,
                R_Index_02,
                R_Index_03,
                R_Middle_01,
                R_Middle_02,
                R_Middle_03,
                R_Ring_01,
                R_Ring_02,
                R_Ring_03,
                R_Pinky_01,
                R_Pinky_02,
                R_Pinky_03,

                L_UpperLeg,
                L_LowerLeg,
                L_Foot,
                L_Toes,
                R_UpperLeg,
                R_LowerLeg,
                R_Foot,
                R_Toes,

                Neck,
                Head,
                L_Eye,
                R_Eye,
                Jaw;

    #endregion

    void Awake()
    {
        animator = GetComponent<Animator>();

        Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        Spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        Chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        UpperChest = animator.GetBoneTransform(HumanBodyBones.UpperChest);

        L_Shoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        L_UpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        L_LowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        L_Hand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        L_Thumb_01 = animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
        L_Thumb_02 = animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
        L_Thumb_03 = animator.GetBoneTransform(HumanBodyBones.LeftThumbDistal);
        L_Index_01 = animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
        L_Index_02 = animator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
        L_Index_03 = animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal);
        L_Middle_01 = animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
        L_Middle_02 = animator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
        L_Middle_03 = animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
        L_Ring_01 = animator.GetBoneTransform(HumanBodyBones.LeftRingProximal);
        L_Ring_02 = animator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
        L_Ring_03 = animator.GetBoneTransform(HumanBodyBones.LeftRingDistal);
        L_Pinky_01 = animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
        L_Pinky_02 = animator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
        L_Pinky_03 = animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal);

        R_Shoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        R_UpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        R_LowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        R_Hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        R_Thumb_01 = animator.GetBoneTransform(HumanBodyBones.RightThumbProximal);
        R_Thumb_02 = animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
        R_Thumb_03 = animator.GetBoneTransform(HumanBodyBones.RightThumbDistal);
        R_Index_01 = animator.GetBoneTransform(HumanBodyBones.RightIndexProximal);
        R_Index_02 = animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
        R_Index_03 = animator.GetBoneTransform(HumanBodyBones.RightIndexDistal);
        R_Middle_01 = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        R_Middle_02 = animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
        R_Middle_03 = animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
        R_Ring_01 = animator.GetBoneTransform(HumanBodyBones.RightRingProximal);
        R_Ring_02 = animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
        R_Ring_03 = animator.GetBoneTransform(HumanBodyBones.RightRingDistal);
        R_Pinky_01 = animator.GetBoneTransform(HumanBodyBones.RightLittleProximal);
        R_Pinky_02 = animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
        R_Pinky_03 = animator.GetBoneTransform(HumanBodyBones.RightLittleDistal);

        L_UpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        L_LowerLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        L_Foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        L_Toes = animator.GetBoneTransform(HumanBodyBones.LeftToes);
        R_UpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        R_LowerLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        R_Foot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        R_Toes = animator.GetBoneTransform(HumanBodyBones.RightToes);

        Neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        Head = animator.GetBoneTransform(HumanBodyBones.Head);
        L_Eye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
        R_Eye= animator.GetBoneTransform(HumanBodyBones.RightEye);
        Jaw = animator.GetBoneTransform(HumanBodyBones.Jaw);
    }
}
