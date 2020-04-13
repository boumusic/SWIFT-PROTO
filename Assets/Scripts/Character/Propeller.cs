using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Propulsion
{
    public float strength = 5f;
    public AnimationCurve curve;
    public float duration = 1f;
    public float gravity = 1f;
    public Vector3 direction = Vector3.one;
    public int priority = 0;


    private Vector3 chosenDir;
    private float progress = 0f;
    private bool started = false;
    private Action action;
    private Vector3 vector;
    private Propeller propeller;

    public Propulsion(Propulsion copy)
    {
        this.strength = copy.strength;
        this.curve = copy.curve;
        this.duration = copy.duration;
        this.gravity = copy.gravity;
        this.direction = copy.direction;
        this.priority = copy.priority;
    }

    public void Start(Action action = null, Propeller propeller = null)
    {
        Start(Vector3.zero ,action, propeller);
    }

    public void Start(Vector3 dir, Action action = null, Propeller propeller = null)
    {
        progress = 0f;
        this.action = action;
        this.propeller = propeller;
        if (dir.magnitude != 0) chosenDir = dir;
        else chosenDir = direction;
        started = true;
    }

    public void Update()
    {
        if (started)
        {
            if (progress < 1f)
            {
                progress += Time.deltaTime / duration;
                vector = chosenDir * strength * curve.Evaluate(progress);
            }

            else
            {
                action?.Invoke();
                started = false;
                propeller?.RemovePropulsion(this);
            }
        }
    }

    public Vector3 Vector => vector;
}

public class Propeller : MonoBehaviour
{
    public List<Propulsion> propulsions = new List<Propulsion>();

    private void Update()
    {
        for (int i = 0; i < propulsions.Count; i++)
        {
            propulsions[i].Update();
        }
    }
    
    public void RegisterPropulsion(Propulsion propulsion, Action action = null)
    {
        RegisterPropulsion(Vector3.zero, propulsion, action);
    }

    public void RegisterPropulsion(Vector3 dir, Propulsion propulsion, Action action = null)
    {
        Propulsion copy = new Propulsion(propulsion);
        
        propulsions.Add(copy);
        copy.Start(dir, action, this);
    }

    public void RemovePropulsion(Propulsion propulsion)
    {
        Debug.Log("Remove");
        propulsions.Remove(propulsion);
    }

    public Vector3 Velocity()
    {
        if (propulsions.Count == 0)
        {
            return Vector3.zero;
        }

        else
        {
            Propulsion propulsion = propulsions[0];
            for (int i = 0; i < propulsions.Count; i++)
            {
                if (!(propulsions[i].priority < propulsion.priority))
                {
                    propulsion = propulsions[i];
                }
            }
            //Debug.Log(propulsion.Vector);
            return propulsion.Vector;
        }
    }
    public bool IsPropelling => propulsions.Count > 0;
}
