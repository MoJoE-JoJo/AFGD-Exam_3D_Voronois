using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public GameObject spawnPoint;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        player.GetComponent<PlayerControls>().canMove = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerControls>().canMove = false;
            other.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = spawnPoint.transform.position;
            other.gameObject.transform.rotation = spawnPoint.transform.rotation;
            other.GetComponent<CharacterController>().enabled = true;

        }
    }
}
