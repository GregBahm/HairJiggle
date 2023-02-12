using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class HairJiggle : MonoBehaviour
{
    [SerializeField]
    private float gain;
    [SerializeField] 
    private float decay;

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
    private ComputeBuffer hairMeshPoints;
    private const int hairPositionsStride = sizeof(float) * 3;
    private ComputeBuffer hairPositions;
    private const int hairVelocityStride = sizeof(float) * 3;
    private ComputeBuffer hairVelocity;

    private int velocityComputeKernel;
    private const int dispatchSize = 128;
    private int dispatchGroups;

    private Matrix4x4 lastHeadBonePos;

    private void Start()
    {
        hairVertCount = hairMesh.vertexCount;
        hairMeshPoints = GetHairMeshPointsBuffer();
        hairPositions = new ComputeBuffer(hairVertCount, hairPositionsStride);
        hairVelocity = new ComputeBuffer(hairVertCount, hairVelocityStride);

        velocityComputeKernel = velocityCompute.FindKernel("VelocityCompute");
        dispatchGroups = Mathf.CeilToInt((float)hairVertCount / dispatchSize);

        lastHeadBonePos = headBone.localToWorldMatrix;
    }

    private void Update()
    {
        velocityCompute.SetMatrix("_HeadBone", headBone.localToWorldMatrix);
        velocityCompute.SetMatrix("_LastHeadBone", lastHeadBonePos);
        velocityCompute.SetFloat("_Gain", gain);
        velocityCompute.SetFloat("_Decay", decay);
        velocityCompute.SetBuffer(velocityComputeKernel, "_HairMeshPoints", hairMeshPoints);
        velocityCompute.SetBuffer(velocityComputeKernel, "_HairVelocity", hairVelocity);
        velocityCompute.SetBuffer(velocityComputeKernel, "_HairPosition", hairPositions);
        velocityCompute.Dispatch(velocityComputeKernel, dispatchGroups, 1, 1);

        hairMat.SetBuffer("_HairPosition", hairPositions);

        lastHeadBonePos = headBone.localToWorldMatrix;
    }

    private void OnDestroy()
    {
        hairMeshPoints.Release();
        hairPositions.Release();
    }

    private ComputeBuffer GetHairMeshPointsBuffer()
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
