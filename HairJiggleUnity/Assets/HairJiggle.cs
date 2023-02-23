using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HairJiggle : MonoBehaviour
{
    [SerializeField]
    private Transform hairJointRoot;
    Dictionary<Transform, HairJoint> hairJoints;

    [SerializeField]
    private SkinnedMeshRenderer hair;

    [SerializeField]
    private Transform cranium;

    [SerializeField]
    private Transform hairProxyTransform;

    private Material hairMat;

    [SerializeField]
    private float spring;

    [SerializeField]
    private float damper;

    [SerializeField]
    private float mass;

    [SerializeField]
    private float drag;

    [SerializeField]
    private float settle;

    private const int basePointStride = sizeof(float) * 3;
    private ComputeBuffer hairMeshPoints;

    private void Start()
    {
        InitializeHairJoints();
        hairMat = hair.material;
        hairMeshPoints = GetHairMeshPointsBuffer();
    }

    private void InitializeHairJoints()
    {
        hairJoints = new Dictionary<Transform, HairJoint>(); 
        Rigidbody rootRigid = hairJointRoot.GetComponent<Rigidbody>();
        foreach (Transform boneJoint in hairJointRoot.GetComponentsInChildren<Transform>()
            .Where(item => item != hairJointRoot))
        {
            HairJoint hairJoint = new HairJoint(boneJoint, hairJointRoot);
            hairJoints.Add(boneJoint, hairJoint);
        }
        foreach (HairJoint item in hairJoints.Values)
        {
            Transform parentBone = item.Bone.parent;
            Rigidbody body = hairJoints.ContainsKey(parentBone) ? hairJoints[parentBone].RigidBody : rootRigid;
            item.SpringJoint.connectedBody = body;
        }
    }

    private void Update()
    {
        hairMat.SetVector("_CraniumCenter", cranium.position);
        hairMat.SetFloat("_CraniumRadius", cranium.localScale.x * .5f);
        hairMat.SetBuffer("_HairPosition", hairMeshPoints);
        hairMat.SetMatrix("_RootBone", hairProxyTransform.localToWorldMatrix);
    }

    private void FixedUpdate()
    {
        foreach (HairJoint item in hairJoints.Values)
        {
            item.RigidBody.drag = drag;
            item.RigidBody.mass = mass;
            item.SpringJoint.spring = spring;
            item.SpringJoint.damper = damper;
            item.DynamicObject.position = Vector3.Lerp(item.DynamicObject.position, item.RestPosition.position, settle);
            item.DynamicObject.rotation = Quaternion.Lerp(item.DynamicObject.rotation, item.RestPosition.rotation, settle);
            item.Bone.position = item.DynamicObject.position;
            item.Bone.rotation = item.DynamicObject.rotation;
        }
    }

    private ComputeBuffer GetHairMeshPointsBuffer()
    {
        int hairVertCount = hair.sharedMesh.vertexCount;
        ComputeBuffer ret = new ComputeBuffer(hairVertCount, basePointStride);
        Vector3[] data = new Vector3[hairVertCount];

        for (int i = 0; i < hairVertCount; i++)
        {
            data[i] = hair.sharedMesh.vertices[i];
        }
        ret.SetData(data);

        return ret;
    }

    private void OnDestroy()
    {
        hairMeshPoints.Release();
    }

    private class HairJoint
    {
        public Transform Bone { get; private set; }
        public Transform RestPosition { get; private set; }
        public Transform DynamicObject { get; private set; }

        public SpringJoint SpringJoint { get; private set; }
        public Rigidbody RigidBody { get; private set; }

        public HairJoint(Transform bone, Transform hairRoot)
        {
            Bone = bone;
            RestPosition = new GameObject(bone.gameObject.name + " rest").transform;
            RestPosition.rotation = bone.rotation;
            RestPosition.position = bone.position;
            RestPosition.SetParent(hairRoot, true);

            GameObject dynamicObj = new GameObject(bone.gameObject.name + " dynamics");
            DynamicObject = dynamicObj.transform;
            DynamicObject.rotation = bone.rotation;
            DynamicObject.position = bone.position;
            RigidBody = dynamicObj.AddComponent<Rigidbody>();
            SpringJoint = dynamicObj.AddComponent<SpringJoint>();
        }
    }
}
