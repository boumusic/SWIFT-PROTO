using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pataya.QuikFeedback;

public class CharacterFeedbacks : MonoBehaviour
{
    public QuikFeedback[] quikFeedbacks;

    public void Play(string name)
    {
        for (int i = 0; i < quikFeedbacks.Length; i++)
        {
            if(quikFeedbacks[i].feedbackName == name)
            {
                quikFeedbacks[i].Play();
            }
        }
    }
}
