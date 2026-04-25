using UnityEngine;

public class BitcrusherTest : MonoBehaviour
{
    [Range(0f, 100f)]
    public float BitcrushAmount = 100f;

    void Update()
    {
        AkSoundEngine.SetRTPCValue("BitcrushAmount", BitcrushAmount);
    }
}