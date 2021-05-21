using UnityEngine;

public class Fairy : MonoBehaviour
{
    [SerializeField] float speed = 100.0f;
    [SerializeField] float initialSpeed = 1000f;
    float realSpeed;
    float followSpeed;
    ControlQueue player;
    bool returnPressed = false;
    Animator fairyAnimator;

    // Start is called before the first frame update
    void Start()
    {
        fairyAnimator = GetComponent<Animator>();
        player = FindObjectOfType<ControlQueue>();
        realSpeed = initialSpeed *Time.deltaTime;
        followSpeed = speed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {

        if (PlayerPrefs.GetInt("Practice") == 0)
        {
            if (Input.GetKey(KeyCode.Return) && !returnPressed)
            {
                fairyAnimator.enabled = false;
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, realSpeed);
                returnPressed = true;
                Debug.Log("Setting Trigger");

            }

            else if (returnPressed)
            {

                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, followSpeed);
            }
        }
        
    }


}
