using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBones : MonoBehaviour
{
    [SerializeField]
    private Transform[] chain;
    [SerializeField]
    private float spring;
    [SerializeField]
    private float damper;
    [SerializeField]
    private float drag;
    [SerializeField]
    private float angularDrag;

    private Rigidbody[] bodies;
    private SpringJoint[] joints;

    private void Start()
    {
        bodies = new Rigidbody[chain.Length];
        joints = new SpringJoint[chain.Length - 1];

        for (int i = 0; i < chain.Length; i++)
        {
            Rigidbody body = chain[i].gameObject.AddComponent<Rigidbody>();
            body.constraints = i == 0 ? RigidbodyConstraints.FreezePosition : RigidbodyConstraints.None;
            bodies[i] = body;
        }

        for (int i = 0; i < chain.Length - 1; i++)
        {
            SpringJoint springJoint = chain[i + 1].gameObject.AddComponent<SpringJoint>();
            springJoint.connectedBody = bodies[i];
            joints[i] = springJoint;
        }
    }

    private void Update()
    {
        foreach (var item in joints)
        {
            item.spring = spring;
            item.damper = damper;
        }
        foreach (var item in bodies)
        {
            item.drag = drag;
            item.angularDrag = angularDrag;
        }
    }
}
