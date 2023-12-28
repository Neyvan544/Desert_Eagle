using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiePerWave = 5;
    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f;

    public int currentWave = 0;
    public float waveCooldown = 10.0f;

    public bool inCooldown;
    public float cooldownCounter = 0;

    public List<Enemy> currentZombieAlive;

    public GameObject zombiePrefab;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI cooldownCounterUI;

    public TextMeshProUGUI currentWaveUI;

    private void Start()
    {
        currentZombiesPerWave = initialZombiePerWave;

        GlobalReferences.Instance.waveNumber = currentWave;

        StartNextWave();
    }

    private void StartNextWave()
    {
       currentZombieAlive.Clear();
        currentWave++;
        GlobalReferences.Instance.waveNumber = currentWave;
        currentWaveUI.text = "Wave: " + currentWave.ToString();
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for(int i = 0; i < currentZombiesPerWave; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-1f,1f), 0f, Random.Range(-1f,1f));
            Vector3 spawnPosition = transform.position + spawnOffset;  
            
            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            Enemy enemyScript = zombie.GetComponent<Enemy>();

            currentZombieAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update()
    {
        List<Enemy> zombieToRemove = new List<Enemy>();
        foreach(Enemy zombie  in currentZombieAlive)
        {
            if(zombie.isDead)
            {
                zombieToRemove.Add(zombie);
            }
        }

        foreach (Enemy zombie in zombieToRemove)
        {
            currentZombieAlive.Remove(zombie);
        }

        zombieToRemove.Clear();

        if(currentZombieAlive.Count ==  0 && inCooldown == false)
        {
            StartCoroutine(WaveCooldown());
        }

        if(inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            cooldownCounter = waveCooldown;
        }

        cooldownCounterUI.text = cooldownCounter.ToString("f0");
    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;
        waveOverUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;
        waveOverUI.gameObject.SetActive(false);


        currentZombiesPerWave *= 2;
        StartNextWave();
    }
}
