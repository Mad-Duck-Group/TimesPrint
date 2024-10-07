using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySoundFX(SoundFXTypes.CoinCollect, out _);
            StarManager.Instance.CollectStar();  // เรียกฟังก์ชัน CollectStar เมื่อชนกับไอเท็มดาว
            gameObject.SetActive(false);
        }
    }
}
