using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataConsent : MonoBehaviour
{
    public void ConsentGiven(){
        GameSession.Instance.ConsentGiven();
    }

    public void ConsentNotGiven(){
        GameSession.Instance.ConsentNotGiven();
    }
}
