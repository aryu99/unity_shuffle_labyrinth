using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class agentController : Agent
{
    private float moveSpeed = 6f;
    private float distanceThreshold = 0.5f;
    private float velThreshold = 0.5f;
    private float initialStateBuffer_dist = 0.15f;
    private float initialStateBuffer_vel = 0.15f;
    [SerializeField] private Transform targetTransform;
    private Rigidbody rb;
    private float distanceToTarget;
    private float distanceToSubTaskTarget;
    private float distanceinitToSubTaskTarget;
    private float normdistanceToSubTaskTarget;
    CustomSideChannel sideChannel;
    private int currentTask;
    private int currentSubTask;
    private Vector3 initLocation;
    private Vector3 targetLocation;
    private Vector3 subTaskTargetLocation;
    private Vector3 globalInitLocation;
    private Vector3 subTaskInitLocation;
    public int room_ID;

    private List<int> flippedRooms = new List<int>();    
    private List<int> mirrorRooms = new List<int>();
    

    // [SerializeField] private Transform s1;

    private Dictionary<int, (Vector3, Vector3)> taskDescriptions;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Dictionary of task descriptions. 
        // The key corresponds to the index of the (sub)task.
        // The value consists of two vectors. The first is the task's initial
        // location, the second is the task's target location.

        flippedRooms.Add(17);
        flippedRooms.Add(18);
        flippedRooms.Add(19);

        mirrorRooms.Add(3);
        mirrorRooms.Add(4);
        mirrorRooms.Add(5);
        mirrorRooms.Add(6);
        mirrorRooms.Add(7);
        mirrorRooms.Add(8);
        mirrorRooms.Add(9);
        mirrorRooms.Add(10);
        mirrorRooms.Add(11);
        mirrorRooms.Add(12);
        mirrorRooms.Add(13);
        mirrorRooms.Add(14);
        mirrorRooms.Add(15);
        mirrorRooms.Add(16);

        taskDescriptions = new Dictionary<int, (Vector3, Vector3)>();

        // Room #1 (TL)
        if(room_ID == 1){
        taskDescriptions.Add(-1, (new Vector3(1.11f, 0.5f, -1.08f), new Vector3(1f, 0.5f, -18.5f)));
        taskDescriptions.Add(0, (new Vector3(1.11f, 0.5f, -1.08f), new Vector3(5.97f, 0.5f, -3.06f)));
        taskDescriptions.Add(1, (new Vector3(1.11f, 0.5f, -1.08f), new Vector3(2.71f, 0.5f, -5.9f)));
        taskDescriptions.Add(2, (new Vector3(5.97f, 0.5f, -3.06f), new Vector3(17.33f, 0.5f, -6.06f)));
        taskDescriptions.Add(3, (new Vector3(5.97f, 0.5f, -8.12f), new Vector3(2.71f, 0.5f, -5.9f)));
        taskDescriptions.Add(4, (new Vector3(2.71f, 0.5f, -5.9f), new Vector3(2.71f, 0.5f, -13.95f)));
        taskDescriptions.Add(5, (new Vector3(2.71f, 0.5f, -13.95f), new Vector3(2.71f, 0.5f, -5.9f)));
        taskDescriptions.Add(6, (new Vector3(5.97f, 0.5f, -8.12f), new Vector3(2.71f, 0.5f, -13.95f)));
        taskDescriptions.Add(7, (new Vector3(2.71f, 0.5f, -13.95f), new Vector3(5.97f, 0.5f, -8.12f)));
        taskDescriptions.Add(8, (new Vector3(2.71f, 0.5f, -13.95f), new Vector3(14.12f, 0.5f, -11.86f)));
        taskDescriptions.Add(9, (new Vector3(14.12f, 0.5f, -11.86f), new Vector3(2.71f, 0.5f, -13.95f)));
        taskDescriptions.Add(10, (new Vector3(17.33f, 0.5f, -13.91f), new Vector3(17.33f, 0.5f, -6.06f)));
        taskDescriptions.Add(11, (new Vector3(17.33f, 0.5f, -6.06f), new Vector3(17.33f, 0.5f, -13.91f)));
        taskDescriptions.Add(12, (new Vector3(17.33f, 0.5f, -6.06f), new Vector3(14.12f, 0.5f, -11.86f)));
        taskDescriptions.Add(13, (new Vector3(14.12f, 0.5f, -11.86f), new Vector3(17.33f, 0.5f, -6.06f)));
        taskDescriptions.Add(14, (new Vector3(17.33f, 0.5f, -6.06f), new Vector3(5.97f, 0.5f, -8.12f)));
        taskDescriptions.Add(15, (new Vector3(5.97f, 0.5f, -8.12f), new Vector3(17.33f, 0.5f, -6.06f)));
        taskDescriptions.Add(16, (new Vector3(14.12f, 0.5f, -11.86f), new Vector3(17.33f, 0.5f, -13.91f)));
        taskDescriptions.Add(17, (new Vector3(17.33f, 0.5f, -13.91f), new Vector3(5.97f, 0.5f, -16.77f)));
        taskDescriptions.Add(18, (new Vector3(2.71f, 0.5f, -13.95f), new Vector3(1.0f, 0.5f, -18.5f)));
        taskDescriptions.Add(19, (new Vector3(5.97f, 0.5f, -16.77f), new Vector3(1.0f, 0.5f, -18.5f)));
        }

        // Room #2 (TR)
        else if(room_ID == 2){
        taskDescriptions.Add(-1, (new Vector3(1.11f + 20f, 0.5f, -1.08f), new Vector3(1f + 20f, 0.5f, -18.5f)));
        taskDescriptions.Add(0, (new Vector3(1.11f + 20f, 0.5f, -1.08f), new Vector3(5.97f + 20f, 0.5f, -3.06f)));
        taskDescriptions.Add(1, (new Vector3(1.11f + 20f, 0.5f, -1.08f), new Vector3(2.71f + 20f, 0.5f, -5.9f)));
        taskDescriptions.Add(2, (new Vector3(5.97f + 20f, 0.5f, -3.06f), new Vector3(17.33f + 20f, 0.5f, -6.06f)));
        taskDescriptions.Add(16, (new Vector3(5.97f + 20f, 0.5f, -8.12f), new Vector3(2.71f + 20f, 0.5f, -5.9f)));
        taskDescriptions.Add(10, (new Vector3(2.71f + 20f, 0.5f, -5.9f), new Vector3(2.71f + 20f, 0.5f, -13.95f)));
        taskDescriptions.Add(11, (new Vector3(2.71f + 20f, 0.5f, -13.95f), new Vector3(2.71f + 20f, 0.5f, -5.9f)));
        taskDescriptions.Add(13, (new Vector3(5.97f + 20f, 0.5f, -8.12f), new Vector3(2.71f + 20f, 0.5f, -13.95f)));
        taskDescriptions.Add(12, (new Vector3(2.71f + 20f, 0.5f, -13.95f), new Vector3(5.97f + 20f, 0.5f, -8.12f)));
        taskDescriptions.Add(14, (new Vector3(2.71f + 20f, 0.5f, -13.95f), new Vector3(14.12f + 20f, 0.5f, -11.86f)));
        taskDescriptions.Add(15, (new Vector3(14.12f + 20f, 0.5f, -11.86f), new Vector3(2.71f + 20f, 0.5f, -13.95f)));
        taskDescriptions.Add(4, (new Vector3(17.33f + 20f, 0.5f, -13.91f), new Vector3(17.33f + 20f, 0.5f, -6.06f)));
        taskDescriptions.Add(5, (new Vector3(17.33f + 20f, 0.5f, -6.06f), new Vector3(17.33f + 20f, 0.5f, -13.91f)));
        taskDescriptions.Add(7, (new Vector3(17.33f + 20f, 0.5f, -6.06f), new Vector3(14.12f + 20f, 0.5f, -11.86f)));
        taskDescriptions.Add(6, (new Vector3(14.12f + 20f, 0.5f, -11.86f), new Vector3(17.33f + 20f, 0.5f, -6.06f)));
        taskDescriptions.Add(8, (new Vector3(17.33f + 20f, 0.5f, -6.06f), new Vector3(5.97f + 20f, 0.5f, -8.12f)));
        taskDescriptions.Add(9, (new Vector3(5.97f + 20f, 0.5f, -8.12f), new Vector3(17.33f + 20f, 0.5f, -6.06f)));
        taskDescriptions.Add(3, (new Vector3(14.12f + 20f, 0.5f, -11.86f), new Vector3(17.33f + 20f, 0.5f, -13.91f)));
        taskDescriptions.Add(17, (new Vector3(17.33f + 20f, 0.5f, -13.91f), new Vector3(5.97f + 20f, 0.5f, -16.77f)));
        taskDescriptions.Add(18, (new Vector3(2.71f + 20f, 0.5f, -13.95f), new Vector3(1.0f + 20f, 0.5f, -18.5f)));
        taskDescriptions.Add(19, (new Vector3(5.97f + 20f, 0.5f, -16.77f), new Vector3(1.0f + 20f, 0.5f, -18.5f)));
        }

        // Room #3 (BL)
        else if(room_ID == 3){
        taskDescriptions.Add(-1, (new Vector3(1.11f, 0.5f, -1.08f - 20f), new Vector3(19f, 0.5f, -18.0f - 20f)));
        taskDescriptions.Add(0, (new Vector3(1.11f, 0.5f, -1.08f - 20f), new Vector3(5.97f, 0.5f, -3.06f - 20f)));
        taskDescriptions.Add(1, (new Vector3(1.11f, 0.5f, -1.08f - 20f), new Vector3(2.71f, 0.5f, -5.9f - 20f)));
        taskDescriptions.Add(2, (new Vector3(5.97f, 0.5f, -3.06f - 20f), new Vector3(17.33f, 0.5f, -6.06f - 20f)));
        taskDescriptions.Add(3, (new Vector3(5.97f, 0.5f, -8.12f - 20f), new Vector3(2.71f, 0.5f, -5.9f - 20f)));
        taskDescriptions.Add(4, (new Vector3(2.71f, 0.5f, -25.59f), new Vector3(2.71f, 0.5f, -13.57f - 20f)));
        taskDescriptions.Add(5, (new Vector3(2.71f, 0.5f, -13.95f - 20f), new Vector3(2.71f, 0.5f, -5.9f - 20f)));
        taskDescriptions.Add(6, (new Vector3(5.97f, 0.5f, -8.12f - 20f), new Vector3(2.71f, 0.5f, -13.95f - 20f)));
        taskDescriptions.Add(7, (new Vector3(2.71f, 0.5f, -13.95f - 20f), new Vector3(5.97f, 0.5f, -8.12f - 20f)));
        taskDescriptions.Add(8, (new Vector3(2.79f, 0.5f, -13.57f - 20f), new Vector3(14.12f, 0.5f, -31.22f)));
        taskDescriptions.Add(9, (new Vector3(14.12f, 0.5f, -11.86f - 20f), new Vector3(2.71f, 0.5f, -13.95f - 20f)));
        taskDescriptions.Add(10, (new Vector3(17.29f, 0.5f, -13.46f - 20f), new Vector3(17.29f, 0.5f, -5.59f - 20f)));
        taskDescriptions.Add(11, (new Vector3(17.33f, 0.5f, -6.06f - 20f), new Vector3(17.33f, 0.5f, -13.91f - 20f)));
        taskDescriptions.Add(12, (new Vector3(17.33f, 0.5f, -6.06f - 20f), new Vector3(14.12f, 0.5f, -11.86f - 20f)));
        taskDescriptions.Add(13, (new Vector3(14.12f, 0.5f, -11.86f - 20f), new Vector3(17.33f, 0.5f, -6.06f - 20f)));
        taskDescriptions.Add(14, (new Vector3(17.33f, 0.5f, -6.06f - 20f), new Vector3(5.97f, 0.5f, -8.12f - 20f)));
        taskDescriptions.Add(15, (new Vector3(5.97f, 0.5f, -8.12f - 20f), new Vector3(17.33f, 0.5f, -6.06f - 20f)));
        taskDescriptions.Add(16, (new Vector3(14.12f, 0.5f, -31.22f), new Vector3(17.33f, 0.5f, -13.91f - 20f)));
        taskDescriptions.Add(17, (new Vector3(2.79f, 0.5f, -13.57f - 20f), new Vector3(14.04f, 0.5f, -36.24f)));
        taskDescriptions.Add(18, (new Vector3(17.3f, 0.5f, -33.5f), new Vector3(19.0f, 0.5f, -38.0f)));
        taskDescriptions.Add(19, (new Vector3(14.2f, 0.5f, -36.3f), new Vector3(19.0f, 0.5f, -38.0f)));
        }        

        // Room #4 (BR) 
        else if(room_ID == 4){
        taskDescriptions.Add(-1, (new Vector3(1.11f + 20f, 0.5f, -1.08f - 20f), new Vector3(19.03f + 20f, 0.5f, -38.01f)));
        taskDescriptions.Add(0, (new Vector3(1.11f + 20f, 0.5f, -1.08f - 20f), new Vector3(5.97f + 20f, 0.5f, -22.48f)));
        taskDescriptions.Add(1, (new Vector3(1.11f + 20f, 0.5f, -1.08f - 20f), new Vector3(2.71f + 20f, 0.5f, -5.9f - 20f)));
        taskDescriptions.Add(2, (new Vector3(5.97f + 20f, 0.5f, -22.48f), new Vector3(17.26f + 20f, 0.5f, -25.48f)));
        taskDescriptions.Add(16, (new Vector3(26.06f, 0.5f, -27.77f), new Vector3(2.71f + 20f, 0.5f, -5.9f - 20f)));
        taskDescriptions.Add(10, (new Vector3(2.74f + 20f, 0.5f, -5.48f - 20f), new Vector3(2.64f + 20f, 0.5f, -13.52f - 20f)));
        taskDescriptions.Add(11, (new Vector3(2.74f + 20f, 0.5f, -13.52f - 20f), new Vector3(2.71f + 20f, 0.5f, -5.9f - 20f)));
        taskDescriptions.Add(13, (new Vector3(26.06f, 0.5f, -27.77f), new Vector3(2.71f + 20f, 0.5f, -13.52f - 20f)));
        taskDescriptions.Add(12, (new Vector3(2.71f + 20f, 0.5f, -13.52f - 20f), new Vector3(26.06f, 0.5f, -27.77f)));
        taskDescriptions.Add(14, (new Vector3(2.71f + 20f, 0.5f, -13.52f - 20f), new Vector3(34.01f, 0.5f, -31.41f)));
        taskDescriptions.Add(15, (new Vector3(34.01f, 0.5f, -31.41f), new Vector3(2.71f + 20f, 0.5f, -13.52f - 20f)));
        taskDescriptions.Add(4, (new Vector3(37.26f, 0.5f, -33.52f), new Vector3(37.26f, 0.5f, -25.48f)));
        taskDescriptions.Add(5, (new Vector3(37.26f, 0.5f, -25.48f), new Vector3(37.26f, 0.5f, -33.52f)));
        taskDescriptions.Add(7, (new Vector3(37.26f, 0.5f, -25.48f), new Vector3(34.01f, 0.5f, -31.41f)));
        taskDescriptions.Add(6, (new Vector3(34.01f, 0.5f, -31.41f), new Vector3(17.33f + 20f, 0.5f, -6.06f - 20f)));
        taskDescriptions.Add(8, (new Vector3(37.26f, 0.5f, -25.48f), new Vector3(26.06f, 0.5f, -27.77f)));
        taskDescriptions.Add(9, (new Vector3(26.06f, 0.5f, -27.77f), new Vector3(37.26f, 0.5f, -25.48f)));
        taskDescriptions.Add(3, (new Vector3(34.01f, 0.5f, -31.41f), new Vector3(37.26f, 0.5f, -33.52f)));
        taskDescriptions.Add(17, (new Vector3(2.64f + 20f, 0.5f, -13.52f - 20f), new Vector3(14.01f + 20f, 0.5f, -36.19f)));
        taskDescriptions.Add(18, (new Vector3(17.26f + 20f, 0.5f, -33.52f), new Vector3(19.03f + 20f, 0.5f, -38.01f)));
        taskDescriptions.Add(19, (new Vector3(14.01f + 20f, 0.5f, -36.19f), new Vector3(19.03f + 20f, 0.5f, -38.01f)));
        }        


        // Set the current task to the value representing the overall task
        currentTask = -1;
        SetCurrentTask(currentTask);

        // Set the current subtask to the value representing the overall task
        currentSubTask = -1;
        SetCurrentSubTask(currentSubTask);

        // Instantiate and register the custom side channel
        sideChannel = new CustomSideChannel();
        SideChannelManager.RegisterSideChannel(sideChannel);
        sideChannel.MessageToPass += OnMessageReceived;

    }

    public void OnMessageReceived(object sender, MessageEventArgs msg)
    {
        // Message should be two integers separated by a comma.
        // The first integer corresponds to the current task to solve.
        // The second integer corresponds to the current sub-task to solve.
        string message = msg.message;
        string[] split_message = message.Split(',');

        currentTask = System.Int32.Parse(split_message[0]);
        currentSubTask = System.Int32.Parse(split_message[1]);

        SetCurrentTask(currentTask);
        SetCurrentSubTask(currentSubTask);
        // Debug.Log("current Task" + currentTask);
        // Debug.Log("current Subtask" + currentSubTask);
    }

    public override void OnEpisodeBegin()
    {
        // Get a random initial velocity
        float vel_r = (velThreshold + initialStateBuffer_vel) * Random.Range(0f, 1f);
        float vel_theta = 2f * Mathf.PI * Random.Range(0f, 1f);
        float vel_x = vel_r * Mathf.Cos(vel_theta);
        float vel_z = vel_r * Mathf.Sin(vel_theta);

        // rb.velocity = new Vector3(0, 0, 0);
        rb.velocity = new Vector3(vel_x, 0, vel_z);

        // Get a random initial position centered around the initial location
        float pos_r = (distanceThreshold + initialStateBuffer_dist) * Random.Range(0f, 1f);
        float pos_theta = 2f * Mathf.PI * Random.Range(0f, 1f);
        float pos_x = initLocation.x + pos_r * Mathf.Cos(pos_theta);
        float pos_z = initLocation.z + pos_r * Mathf.Sin(pos_theta);

        transform.position = new Vector3(pos_x, initLocation.y, pos_z);
    }

    public override void CollectObservations(VectorSensor sensor)
    {   
        // flipped
        if ((flippedRooms.Contains(currentSubTask)) && (room_ID == 3 || room_ID == 4))
        {   
            sensor.AddObservation(transform.position.x - subTaskInitLocation.x);
            sensor.AddObservation(subTaskTargetLocation.x - subTaskInitLocation.x);
            sensor.AddObservation(subTaskInitLocation.z - transform.position.z);
            sensor.AddObservation(subTaskInitLocation.z - subTaskTargetLocation.z);
            sensor.AddObservation(-rb.velocity.x);
            sensor.AddObservation(rb.velocity.z);
        }
        // mirrored
        else if ((mirrorRooms.Contains(currentSubTask)) && (room_ID == 2 || room_ID == 4))
        {
            sensor.AddObservation(transform.position.x - subTaskInitLocation.x);
            sensor.AddObservation(subTaskTargetLocation.x - subTaskInitLocation.x);
            sensor.AddObservation(transform.position.z - subTaskInitLocation.z);
            sensor.AddObservation(subTaskTargetLocation.z - subTaskInitLocation.z);
            sensor.AddObservation(-rb.velocity.x);
            sensor.AddObservation(-rb.velocity.z);
        }
        else
        {
            sensor.AddObservation(subTaskInitLocation.x - transform.position.x);
            sensor.AddObservation(subTaskInitLocation.x - subTaskTargetLocation.x);
            sensor.AddObservation(subTaskInitLocation.z - transform.position.z);
            sensor.AddObservation(subTaskInitLocation.z - subTaskTargetLocation.z);
            sensor.AddObservation(rb.velocity.x);
            sensor.AddObservation(rb.velocity.z);
        }
        
        // sensor.AddObservation(targetTransform.position);
        sensor.AddObservation(Vector3.Distance(transform.position, subTaskTargetLocation));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {      
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // flipped
        if ((flippedRooms.Contains(currentSubTask)) && (room_ID == 3 || room_ID == 4))
        {
            rb.AddForce(new Vector3(-moveX, 0, moveZ) * moveSpeed);
        }
        // mirrored
        else if ((mirrorRooms.Contains(currentSubTask)) && (room_ID == 2 || room_ID == 4))
        {
            rb.AddForce(new Vector3(-moveX, 0, -moveZ) * moveSpeed);
        }
        else
        {
            rb.AddForce(new Vector3(moveX, 0, moveZ) * moveSpeed);
        }

        distanceToTarget = 
            Vector3.Distance(this.transform.position, targetTransform.position);
        // AddReward(-Mathf.Pow(distanceToTarget, 2));

        // float distanceReward =  

        distanceToSubTaskTarget = 
            Vector3.Distance(this.transform.position, subTaskTargetLocation);
        // AddReward(-Mathf.Pow(distanceToSubTaskTarget, 2)*10f);
        AddReward(-distanceToTarget/10000f);

        distanceinitToSubTaskTarget = 
            Vector3.Distance(subTaskInitLocation, subTaskTargetLocation);
        
        // normdistanceToSubTaskTarget = Mathf.Cos(((distanceToSubTaskTarget/distanceinitToSubTaskTarget)*90)*Mathf.Deg2Rad);

        // Compute current speed.
        float vel = Mathf.Sqrt((rb.velocity.x * rb.velocity.x) + (rb.velocity.z * rb.velocity.z));
        
        // if (vel > 0.3f)
        // {
        //     AddReward(-0.01f);
        // }
        // else
        // {
        //     AddReward(0.01f);
        // }

        if ((distanceToSubTaskTarget <= distanceThreshold) && (vel < velThreshold))
        {
            sideChannel.SendStringToPython("Completed sub task: " + currentSubTask.ToString());
        }

        if ((distanceToTarget <= distanceThreshold) && (vel < velThreshold))
        {
            SetReward(1f);
            sideChannel.SendStringToPython("Completed task");
            EndEpisode();
        }

        // transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Lava")){
            SetReward(-1f);
            sideChannel.SendStringToPython("Failed task");
            EndEpisode();
        }
    }

    // Set the environment task
    public void SetCurrentTask(int currentTask)
    {
        (initLocation, targetLocation) = taskDescriptions[currentTask];
        targetTransform.position = targetLocation;
    }

    public void SetCurrentSubTask(int currentSubTask)
    {
        (subTaskInitLocation, subTaskTargetLocation) = taskDescriptions[currentSubTask];
    }

    // Defining a basic heuristic for the agent when not learning
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

}


