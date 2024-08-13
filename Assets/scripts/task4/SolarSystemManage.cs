using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    [System.Serializable]
    public class Planet
    {
        public string name;
        public GameObject planetPrefab;  // Prefab for the planet
        public float orbitRadius;        // Radius
        public float size;               // Size of the planet
        public float orbitSpeed;         // Speed of revolution around the Sun
        public float rotationSpeed;      // Speed of rotation around its own axis
        public float orbitEccentricity;  // Eccentricity
        public float axialTilt;          // Axial tilt
        public List<Moon> moons;         // List of moons 
    }

    [System.Serializable]
    public class Moon
    {
        public string name;
        public GameObject moonPrefab;    // Prefab for the moon
        public float orbitRadius;        // Distance from the planet
        public float size;               // Size
        public float orbitSpeed;         // Speed of revolution around the planet
        public float orbitEccentricity;  // Eccentricity
        public LineRenderer orbitLine;   // LineRenderer
    }

    public GameObject sunPrefab;         // Prefab for the Sun
    public float sunSize;                // Size of the Sun
    public LineRenderer orbitLinePrefab; // Prefab for drawing planet orbit lines

    public List<GameObject> planetInstances = new List<GameObject>(); // List of planets
    public Dictionary<Planet, List<GameObject>> moonInstances = new Dictionary<Planet, List<GameObject>>(); // Track moons for each planet

    public List<Planet> planets;         // List of planets to spawn

    public List<GameObject> lineRenderers = new List<GameObject>(); // track all the line Spawned

    void Start()
    {
        // Place the Sun at the center
        PlaceSun();

        // Place each planet and its moons
        foreach (Planet planet in planets)
        {
            PlacePlanet(planet);
        }
    }

    void Update()
    {
        foreach (Planet planet in planets)
        {
            int planetIndex = planets.IndexOf(planet);
            GameObject planetObj = planetInstances[planetIndex];

            // Rotate and revolve each planet
            RotateAndRevolvePlanet(planet, planetObj);

            // Revolve each moon around its planet
            if (moonInstances.TryGetValue(planet, out var moons))
            {
                for (int i = 0; i < moons.Count; i++)
                {
                    RevolveMoon(planet.moons[i], planetObj, moons[i]);
                    UpdateMoonOrbitLine(planet.moons[i], planetObj);
                }
            }
        }
    }

    private void PlaceSun()
    {

        if (sunPrefab == null)
        {
            Debug.LogError("Sun prefab is not assigned!");
            return;
        }

        // Instantiate the Sun at the origin
        GameObject sunObj = Instantiate(sunPrefab);
        sunObj.name = "Sun";
        sunObj.transform.position = Vector3.zero;
    }

    private void PlacePlanet(Planet planet)
    {

        if (planet.planetPrefab == null)
        {
            Debug.LogError($"Planet prefab for {planet.name} is not assigned!");
            return;
        }

        // Instantiate the planet at the specified orbit radius
        GameObject planetObj = Instantiate(planet.planetPrefab);
        planetObj.name = planet.name;
        planetObj.transform.position = new Vector3(planet.orbitRadius, 0, 0);
        planetObj.transform.localScale = Vector3.one * planet.size; 

        //axial tilt
        planetObj.transform.rotation = Quaternion.Euler(0, 0, planet.axialTilt);

        planetInstances.Add(planetObj);

        // Create the orbit line
        CreateOrbitLine(planet);

        //moon list for the planet
        moonInstances[planet] = new List<GameObject>();

        // Place moons for this planet
        foreach (var moon in planet.moons)
        {
            PlaceMoon(moon, planetObj, planet);
        }
    }

    private void PlaceMoon(Moon moon, GameObject planetObj, Planet planet)
    {
        if (moon.moonPrefab == null)
        {
            Debug.LogError($"Moon prefab for {moon.name} is not assigned!");
            return;
        }

        GameObject moonObj = Instantiate(moon.moonPrefab);
        moonObj.name = moon.name;
        moonObj.transform.position = planetObj.transform.position + new Vector3(moon.orbitRadius, 0, 0);
        moonObj.transform.localScale = Vector3.one * moon.size; // Scale the moon

        // Add moon
        moonInstances[planet].Add(moonObj);

        // orbit line
        moon.orbitLine = CreateMoonOrbitLine(moon, planetObj);
    }

    private void RotateAndRevolvePlanet(Planet planet, GameObject planetObj)
    {
        // Rotate planet around its own axis
        planetObj.transform.Rotate(Vector3.up * planet.rotationSpeed * Time.deltaTime, Space.Self);

        // Calculate the position for the elliptical orbit
        float angle = planet.orbitSpeed * Time.time;
        float x = Mathf.Cos(angle) * planet.orbitRadius * (1 + planet.orbitEccentricity);
        float z = Mathf.Sin(angle) * planet.orbitRadius;
        planetObj.transform.position = new Vector3(x, 0, z);
    }

    private void RevolveMoon(Moon moon, GameObject planetObj, GameObject moonObj)
    {
        // Calculate the position for the elliptical orbit
        float angle = moon.orbitSpeed * Time.time;
        float x = Mathf.Cos(angle) * moon.orbitRadius * (1 + moon.orbitEccentricity);
        float z = Mathf.Sin(angle) * moon.orbitRadius;
        moonObj.transform.position = planetObj.transform.position + new Vector3(x, z, 0);
    }

    private void CreateOrbitLine(Planet planet)
    {
        if (orbitLinePrefab == null)
        {
            Debug.LogError("Orbit line prefab is not assigned!");
            return;
        }

        LineRenderer orbitLine = Instantiate(orbitLinePrefab);
        int segments = 100;
        orbitLine.positionCount = segments + 1;
        float angle = 0f;

        lineRenderers.Add(orbitLine.gameObject);

        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * planet.orbitRadius * (1 + planet.orbitEccentricity);
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * planet.orbitRadius;
            orbitLine.SetPosition(i, new Vector3(x, 0, z));
            angle += 360f / segments;
        }

    }

    private LineRenderer CreateMoonOrbitLine(Moon moon, GameObject planetObj)
    {
        if (orbitLinePrefab == null)
        {
            Debug.LogError("Orbit line prefab is not assigned!");
            return null;
        }

        LineRenderer orbitLine = Instantiate(orbitLinePrefab);
        int segments = 100;
        orbitLine.positionCount = segments + 1;

        lineRenderers.Add(orbitLine.gameObject);
        UpdateMoonOrbitLine(moon, planetObj);

        return orbitLine;
    }

    private void UpdateMoonOrbitLine(Moon moon, GameObject planetObj)
    {
        if (moon.orbitLine == null)
            return;

        int segments = 100;
        float angle = 0f;

        moon.orbitLine.positionCount = segments + 1;

        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * moon.orbitRadius * (1 + moon.orbitEccentricity);
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * moon.orbitRadius;
            moon.orbitLine.SetPosition(i, planetObj.transform.position + new Vector3(x, z, 0));
            angle += 360f / segments;
        }
    }
}
