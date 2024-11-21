using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RabbitManager : MonoBehaviour
{

    [SerializeField] List<GameObject> rabbits;
    [SerializeField] GameObject rabbitsSpawner;
    [SerializeField] GameObject rabbitsPlace;
    Animator animator;
    GameObject currentRabbit = null;
    NavMeshAgent navMesh;
    bool finish = false;
    bool arrived = false;
    RecipeManager recipeManager;
    
    

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
        SetDesination(rabbitsPlace);
    }

    void MakeOrder(){
        animator.SetTrigger("Hello");
        if(recipeManager == null){
            recipeManager = FindObjectOfType<RecipeManager>();
        }
        recipeManager.SelectRandomRecipe();
    }

    public async void Renvoyer(){
        animator.SetTrigger("Hello");
        await Task.Delay(1500);
        SetDesination(rabbitsSpawner);
        arrived = false;
        finish = true;
    }
}
