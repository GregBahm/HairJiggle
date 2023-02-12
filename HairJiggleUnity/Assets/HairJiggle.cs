using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class HairJiggle : MonoBehaviour
{
    [SerializeField]
    private float momentumGain;
    [SerializeField] 
    private float momentumDecay;

    [SerializeField]
    private Mesh hairMesh;
    [SerializeField]
    private Transform headBone;
    [SerializeField]
    private ComputeShader velocityCompute;
    [SerializeField]
    private Material hairMat;

    private int hairVertCount; // TODO: handle merged verts
    private const int basePointStride = sizeof(float) * 3;
    private ComputeBuffer hairBasePoints;
    private const int hairMomentumStride = sizeof(float) * 3;
    private ComputeBuffer hairMomentum;

    private int velocityComputeKernel;
    private const int dispatchSize = 128;
    private int dispatchGroups;

    private Matrix4x4 lastHeadBonePos;

    private void Start()
    {
        hairVertCount = hairMesh.vertexCount;
        hairBasePoints = GetHairBasePointsBuffer();
        hairMomentum = new ComputeBuffer(hairVertCount, hairMomentumStride);

        velocityComputeKernel = velocityCompute.FindKernel("VelocityCompute");
        dispatchGroups = Mathf.CeilToInt((float)hairVertCount / dispatchSize);

        lastHeadBonePos = headBone.localToWorldMatrix;
    }

    private void Update()
    {
        velocityCompute.SetMatrix("_HeadBone", headBone.localToWorldMatrix);
        velocityCompute.SetMatrix("_LastHeadBone", lastHeadBonePos);
        velocityCompute.SetFloat("_MomentumGain", momentumGain);
        velocityCompute.SetFloat("_MomentumDecay", momentumDecay);
        velocityCompute.SetBuffer(velocityComputeKernel, "_HairBasePoints", hairBasePoints);
        velocityCompute.SetBuffer(velocityComputeKernel, "_HairMomentum", hairMomentum);
        velocityCompute.Dispatch(velocityComputeKernel, dispatchGroups, 1, 1);

        hairMat.SetBuffer("_HairMomentum", hairMomentum);

        lastHeadBonePos = headBone.localToWorldMatrix;
    }

    private void OnDestroy()
    {
        hairBasePoints.Release();
        hairMomentum.Release();
    }

    private ComputeBuffer GetHairBasePointsBuffer()
    {
        ComputeBuffer ret = new ComputeBuffer(hairVertCount, basePointStride);
        Vector3[] data = new Vector3[hairVertCount];

        for (int i = 0; i < hairVertCount; i++)
        {
            data[i] = hairMesh.vertices[i];
        }
        ret.SetData(data);

        return ret;
    }
}
