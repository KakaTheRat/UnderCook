using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;


public class RabbitManager : MonoBehaviour
{

    [SerializeField] List<GameObject> rabbits;
    [SerializeField] GameObject rabbitsSpawner;
    [SerializeField] GameObject rabbitsPlace;
    [SerializeField] AudioClip successSound;
    [SerializeField] AudioClip orderSound;
    Animator animator;
    GameObject currentRabbit = null;
    NavMeshAgent navMesh;
    bool finish = false;
    bool arrived = false;
    RecipeManager recipeManager;
    AudioSource audioSource;
    
    

    // Start is called before the first frame update
    void Awake()
    {    
        GetNewRabbit();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", navMesh.velocity.magnitude);
        if(navMesh.remainingDistance == 0 && arrived == false){
            arrived = true;
            if(finish == false){
                MakeOrder();
            }else{
                GetNewRabbit();
                finish = false;
                arrived = false;
            }
            
        }
        
    }

    void SetDesination(GameObject _dest){
        navMesh.SetDestination(_dest.transform.position);
    }

    void GetNewRabbit(){
        GameObject newRabbit = rabbits[UnityEngine.Random.Range(0,rabbits.Count)];
        while(newRabbit == currentRabbit){
            newRabbit = rabbits[UnityEngine.Random.Range(0,rabbits.Count)];
        }
        currentRabbit = newRabbit;
        foreach(GameObject rabbit in rabbits){
            if(rabbit != currentRabbit){
                rabbit.SetActive(false);
            }
        }
        currentRabbit.SetActive(true);
        newRabbit.transform.position = rabbitsSpawner.transform.position;
        navMesh = currentRabbit.GetComponent<NavMeshAgent>();
        animator = currentRabbit.GetComponent<Animator>();
        audioSource = currentRabbit.GetComponent<AudioSource>();
        SetDesination(rabbitsPlace);
    }

    void MakeOrder(){
        audioSource.clip = orderSound;
        audioSource.Play();
        animator.SetTrigger("Hello");
        if(recipeManager == null){
            recipeManager = FindObjectOfType<RecipeManager>();
        }
        recipeManager.SelectRandomRecipe();
    }

    public void Renvoyer(){
        audioSource.clip = successSound;
        audioSource.Play();
        animator.SetTrigger("Hello");
        StartCoroutine(Coroutine_WaitThenLog(1.5f,EndRenvoyer));
    }

    void EndRenvoyer(){
        SetDesination(rabbitsSpawner);
        arrived = false;
        finish = true;
    }

    IEnumerator Coroutine_WaitThenLog(float _duration, Action _callback)
    {
        yield return new WaitForSeconds(_duration);
        _callback?.Invoke();
    }
}
