using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeedBoost
{
    public SpeedBoostSettings settings;
    public float CurrentMultiplier =>  (isFinished || settings == null) ? 1 : Mathf.LerpUnclamped(settings.speedMultiplier, 1, settings.curve.Evaluate(progress));
    private float progress = 0f;
    private bool isFinished = false;

    public void Update()
    {
        if (!isFinished && settings != null)
        {
            progress += Time.deltaTime / settings.duration;
            if (progress > 1)
            {
                progress = 1f;
                isFinished = true;
            }
        }
    }
}

public class SpeedBooster : MonoBehaviour
{
    private SpeedBoost boost;

    public void RegisterBoost(SpeedBoostSettings boostSettings)
    {
        SpeedBoost newBoost = new SpeedBoost();
        boost = newBoost;
        boost.settings = boostSettings;
    }

    private void Update()
    {
        boost?.Update();
    }

    public float VelocityMultiplier => boost != null ? boost.CurrentMultiplier : 1;
}
