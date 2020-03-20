using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Springer
{
    /// <summary>
    /// Interpolates a float between its current value and the specified destination with a spring motion.
    /// </summary>
    /// <param name="current">The current value of the float.</param>
    /// <param name="to">The destination value of the float.</param>
    /// <param name="spring">The spring object containing the spring parameters.</param>
    /// <returns></returns>
    public static float Spring(float current, float to, Spring spring)
    {
        return Spring(current, to, ref spring.velocity.x, spring.tightness, spring.damping);
    }

    /// <summary>
    /// Interpolates a float between its current value and the specified destination with a spring motion.
    /// </summary>
    /// <param name="current">The current value of the float.</param>
    /// <param name="to">The destination value of the float.</param>
    /// <param name="velocity">Arbitrary parameter to store the progress of the float.</param>
    /// <param name="tightness">How tight is the spring motion.</param>
    /// <param name="damping">How much damping is applied to the motion.</param>
    /// <returns></returns>
    public static float Spring(float current, float to, ref float velocity, float tightness, float damping)
    {
        velocity += (-(tightness * (current - to)) - (damping * velocity));
        current += velocity;
        return current;
    }

    /// <summary>
    /// Interpolates a vector between its current value and the specified destination with a spring motion.
    /// </summary>
    /// <param name="current">The current value of the vector.</param>
    /// <param name="to">The destination value of the vector.</param>
    /// <param name="spring">The spring object containing the spring parameters.</param>
    /// <returns></returns>
    public static Vector3 Spring(Vector3 current, Vector3 to, Spring spring)
    {
        return Spring(current, to, ref spring.velocity, spring.tightness, spring.damping);
    }

    /// <summary>
    /// Interpolates a vector between its current value and the specified destination with a spring motion.
    /// </summary>
    /// <param name="current">The current value of the vector.</param>
    /// <param name="to">The destination value of the vector.</param>
    /// <param name="velocity">Arbitrary parameter to store the progress of the float.</param>
    /// <param name="tightness">How tight is the spring motion.</param>
    /// <param name="damping">How much damping is applied to the motion.</param>
    /// <returns></returns>
    public static Vector3 Spring(Vector3 current, Vector3 to, ref Vector3 velocity, float tightness, float damping)
    {
        velocity += (-(tightness * (current - to)) - (damping * velocity));
        current += velocity;
        return current;
    }
}

[System.Serializable]
public class Spring
{
    [Range(0f, 1f)] public float tightness = 0.5f;
    [Range(0f, 0.5f)] public float damping = 0.25f;
    [HideInInspector] public Vector3 velocity;
}
