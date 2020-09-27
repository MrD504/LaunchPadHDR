using UnityEngine;
using UnityEngine.SceneManagement;
public class rocket : MonoBehaviour
{
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip hit;
    [SerializeField] AudioClip winJingle;
    [SerializeField] ParticleSystem flames;
    [SerializeField] ParticleSystem explosion;
    [SerializeField] ParticleSystem fireWorks;

    [SerializeField] private float rcsThrust = 100f;
    [SerializeField] private float mainThrust = 1500f;

    
    Rigidbody rigidBody;
    AudioSource audioSource = default;
    
    bool thrustOn = false;

    enum State { Alive, Dying, Transceding };
    State state = State.Alive;

    void Start()
    {
        // Make sure particle hits don't run on start
        explosion.Stop();
        fireWorks.Stop();
        
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter (Collision collision)
    {
        if (state != State.Alive) { return; } // ignore collsions when dead
        audioSource.Stop(); // stop all sounds before playing next sound

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("special friend");
                break;
            case "Finish":
                // Todo: Player must land safely for 5 seconds before
                // level classed as won
                state = State.Transceding;
                Invoke("LoadNextScene", 2f);
                audioSource.PlayOneShot(winJingle);
                fireWorks.Play();
                break;
            default:
                audioSource.PlayOneShot(hit);
                state = State.Dying;
                flames.Stop();
                explosion.Play();
                Invoke("LoadNextScene", 2f);

                break;
        }
    }

    private void LoadNextScene()
    {
        if (state == State.Dying)
        {
            SceneManager.LoadScene(0); // allow for more than 2 levels
        } else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void RespondToThrustInput()
    {
        // can thrust while rotating
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            flames.Stop();
            thrustOn = false;
        }
    }

    private void ApplyThrust()
    {
        thrustOn = true;


        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        flames.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // freeze normal physics control of rotation


        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (!thrustOn)
        {
            rotationThisFrame = rotationThisFrame / 4;
        }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.forward * rotationThisFrame);
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.Rotate(-(Vector3.forward * rotationThisFrame));
            }
        

        rigidBody.freezeRotation = false; // resume normal physics control of rotation
    }

   
}
