using UnityEngine;

public class AudiencePlacer : MonoBehaviour
{
    public GameObject audiencePrefab; // The audience member prefab
    public int numberOfAudienceMembers = 20; // Total number of audience members
    public float radius = 5f; // Radius of the audience placement circle
    public Vector2 arenaCenter = new Vector2(0, 0); // Center of the arena

    void Start()
    {
        if (audiencePrefab == null)
        {
            Debug.LogError("Audience prefab not assigned!");
            return;
        }

        // Loop through each audience member and place them at an even angle
        for (int i = 0; i < numberOfAudienceMembers; i++)
        {
            // Calculate the angle in radians for each audience member
            float angle = i * Mathf.PI * 2 / numberOfAudienceMembers;

            // Calculate the x and y coordinates based on the angle and radius
            float xPos = Mathf.Cos(angle) * radius + arenaCenter.x;
            float yPos = Mathf.Sin(angle) * radius + arenaCenter.y;

            // Set the position as a vector
            Vector3 position = new Vector3(xPos, yPos, 0);

            // Instantiate the audiencePrefab at the calculated position
            GameObject newAudienceMember = Instantiate(audiencePrefab, position, Quaternion.identity);

            // Optional: ensure the prefab doesn't accidentally contain the script (if prefab might have it)
            if (newAudienceMember.GetComponent<AudiencePlacer>())
            {
                Destroy(newAudienceMember.GetComponent<AudiencePlacer>());
            }
        }
    }
}