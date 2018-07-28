using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerFeedack : MonoBehaviour
{

	public AudioSource intersectingAudio;
	public AudioSource invalidPairAudio;

    
	public void FeedbackIntersectingFace()
	{
        intersectingAudio.Play();
	}

    public void FeedbackInvalidPair()
    {
        invalidPairAudio.Play();
    }


}
