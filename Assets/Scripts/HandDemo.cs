/********************************************************************************//**
\file      Hand.cs
\brief     Basic hand impementation.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HandDemo : MonoBehaviour
{
    public const string ANIM_LAYER_NAME_POINT = "Point Layer";
    public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";
    public const string ANIM_PARAM_NAME_FLEX = "Flex";
    public const string ANIM_PARAM_NAME_POSE = "Pose";
    public const float THRESH_COLLISION_FLEX = 0.9f;

    private Animator m_animator = null;

    private int m_animLayerIndexThumb = -1;
    private int m_animLayerIndexPoint = -1;
    private int m_animParamIndexFlex = -1;
    private int m_animParamIndexPose = -1;

    [Range(0, 10)]
    public int m_pose;

    [Range(0, 1)]
    public float m_flex, m_point, m_thumbsUp, m_pinch;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Get animator layer indices by name, for later use switching between hand visuals
        m_animLayerIndexPoint = m_animator.GetLayerIndex(ANIM_LAYER_NAME_POINT);
        m_animLayerIndexThumb = m_animator.GetLayerIndex(ANIM_LAYER_NAME_THUMB);
        m_animParamIndexFlex = Animator.StringToHash(ANIM_PARAM_NAME_FLEX);
        m_animParamIndexPose = Animator.StringToHash(ANIM_PARAM_NAME_POSE);
    }

    private void Update()
    {
        UpdateAnimStates();
    }

    private void UpdateAnimStates()
    {
        m_animator.SetInteger(m_animParamIndexPose, m_pose);

        m_animator.SetFloat(m_animParamIndexFlex, m_flex);

        m_animator.SetLayerWeight(m_animLayerIndexPoint, m_point);

        m_animator.SetLayerWeight(m_animLayerIndexThumb, m_thumbsUp);

        m_animator.SetFloat("Pinch", m_pinch);
    }
}
