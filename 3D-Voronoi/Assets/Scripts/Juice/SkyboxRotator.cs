using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    public GameObject sunDirection;
    public GameObject moonDirection;


    public Material skyboxDay;
    public Material skyboxNight;
    [Range(0, 1)]
    public float timeOfDay = 0;
    public float speed;

    public float dayRotation;
    public float nightRotation;

    Material skyboxMaterial;

    void Start()
    {
        if (skyboxDay != null && skyboxNight != null)
        {
            skyboxMaterial = new Material(skyboxDay);
        }
    }

    void FixedUpdate()
    {
        if (sunDirection != null)
        {
            Shader.SetGlobalVector("GlobalSunDirection", -sunDirection.transform.forward);
        }
        else
        {
            Shader.SetGlobalVector("GlobalSunDirection", Vector3.zero);
        }

        if (moonDirection != null)
        {
            Shader.SetGlobalVector("GlobalMoonDirection", -moonDirection.transform.forward);
        }
        else
        {
            Shader.SetGlobalVector("GlobalMoonDirection", Vector3.zero);
        }

        if (skyboxDay != null && skyboxNight != null)
        {
            timeOfDay += Time.fixedDeltaTime * speed;
            timeOfDay = timeOfDay % 1f;
            //skyboxMaterial.Lerp(skyboxDay, skyboxNight, timeOfDay);
            var vec3 = sunDirection.transform.rotation.eulerAngles;
            vec3 = Vector3.Lerp(new Vector3(vec3.x, dayRotation, vec3.z), new Vector3(vec3.x, nightRotation, vec3.z), timeOfDay);
            sunDirection.transform.rotation = Quaternion.Euler(vec3);
            RenderSettings.skybox = skyboxMaterial;
        }
    }
}
