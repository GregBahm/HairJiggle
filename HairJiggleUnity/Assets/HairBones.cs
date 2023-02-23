using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
    [SerializeField]
    private float mass;

    private Item[] items;

    [SerializeField]
    private float settle;

    private void Start()
    {
        items = new Item[chain.Length];
        for (int i = 0; i < chain.Length; i++)
        {
            items[i] = new Item(chain[i].gameObject);
        }
        foreach (var item in items)
        {
            item.FinishInitialization();
        }
        foreach (var item in items)
        {
            item.Trans.parent = null;
        }
    }

    private void FixedUpdate()
    {
        foreach (var item in items)
        {
            item.Body.drag = drag;
            item.Body.angularDrag = angularDrag;
            item.Body.mass = mass;
            item.Joint.spring = spring;
            item.Joint.damper = damper;
            item.Trans.localPosition = Vector3.Lerp(item.Trans.position, item.BasePos.position, settle);
            item.Trans.localRotation = Quaternion.Lerp(item.Trans.rotation, item.BasePos.rotation, settle);
        }
    }

    private class Item
    {
        public Transform BasePos { get; private set; }
        public Transform Trans { get; private set; }
        public Rigidbody Body { get; private set; }
        public SpringJoint Joint { get; private set; }

        public Item(GameObject obj)
        {
            Trans = obj.transform;
            Body = obj.gameObject.AddComponent<Rigidbody>();
            Joint = obj.gameObject.AddComponent<SpringJoint>();
            BasePos = new GameObject().transform;
            BasePos.parent = Trans.parent;
            BasePos.localPosition = Trans.localPosition;
            BasePos.localRotation = Trans.localRotation;
        }

        public void FinishInitialization()
        {
            Joint.connectedBody = Trans.parent.GetComponent<Rigidbody>();
        }
    }
}
