using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    string _kill = "KilledAnim(Clone)";
    string _bullet = "Bullet_Prefab(Clone)";
    string _shot = "Shot_Particle(Clone)";

    void Start()
    {
    }

    void Update()
    {
        Remove();
    }

    private void Remove()
    {
        // Czyszczê efekty gdy niepotrzebne
        if (GameObject.Find(_kill)) StartCoroutine(OnDestroyDeath(_kill, 1f));
        if (GameObject.Find(_shot)) StartCoroutine(OnDestroyDeath(_shot, 3f)); // TODO: Gry gracz nie strzela a istnieje obiekt particle strza³u wtedy usuwaæ!
        if (GameObject.Find(_bullet)) StartCoroutine(OnDestroyDeath(_bullet, 4f));
    }

    private IEnumerator OnDestroyDeath(string name, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(GameObject.Find(name));
    }
}
