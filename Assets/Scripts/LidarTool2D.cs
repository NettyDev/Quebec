using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Wymagane dla List<>

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class LidarTool2D : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;

    [Header("Lidar Settings (Zasięg)")]
    [Tooltip("The origin point for the raycasts (e.g., Main Camera)")]
    public Transform raycastOrigin;
    
    [Tooltip("Maximum distance of the lidar scan (in meters)")]
    public float scanDistance = 15f;
    
    [Tooltip("Cooldown in seconds between scans")]
    public float scanCooldown = 5f;
    
    [Tooltip("How wide the scan is horizontally in degrees (Field of View)")]
    public float horizontalScanAngle = 100f;

    [Tooltip("How tall the scan is vertically in degrees")]
    public float verticalScanAngle = 60f;

    [Header("Density Settings (Gęstość i Rozkład)")]
    [Tooltip("Bazowa liczba promieni wystrzeliwanych z kamery")]
    public int baseRays = 400;
    
    [Tooltip("WŁĄCZONE: System automatycznie dodaje więcej kropek na dalekich obiektach, aby utrzymać równomierne pokrycie ścian.")]
    public bool adaptiveDensity = true;
    
    [Tooltip("Jak szeroko rozsypują się dodatkowe promienie. Im mniejsza wartość, tym gęstsze klastry.")]
    public float adaptiveSpreadFactor = 0.15f;

    [Header("Wave Effect Settings (Efekt Fali)")]
    [Tooltip("How fast the wave travels (m/s). Lower is slower.")]
    public float waveSpeed = 25f;

    [Header("Visuals (Wygląd)")]
    [Tooltip("Prefab for the red dot. If empty, a primitive sphere will be created dynamically.")]
    public GameObject dotPrefab;
    
    [Tooltip("Material for the primitive dots. Leave empty to use a generated default red material.")]
    public Material dotMaterial;
    
    [Tooltip("How long (in seconds) the dots stay visible before disappearing")]
    public float dotLifetime = 7f;

    [Tooltip("Sound to play when scanning")]
    public AudioClip scanSound;

    // Timer
    private float nextScanTime = 0f;
    private Material generatedRedMaterial;

    // Struktura do przechowywania danych o kropkach zanim fala do nich dotrze
    private struct DotData
    {
        public Vector3 position;
        public Vector3 normal;
        public float distance;
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (raycastOrigin == null && Camera.main != null)
        {
            raycastOrigin = Camera.main.transform;
        }

        if (dotMaterial == null)
        {
            generatedRedMaterial = new Material(Shader.Find("Standard"));
            generatedRedMaterial.color = Color.red;
            generatedRedMaterial.EnableKeyword("_EMISSION");
            generatedRedMaterial.SetColor("_EmissionColor", Color.red * 2f);
        }
    }

    public void SetToolActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        if (isActive) Debug.Log("Informacja: Lidar został aktywowany.");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= nextScanTime)
            {
                UseLidar();
                nextScanTime = Time.time + scanCooldown;
            }
            else
            {
                Debug.Log($"Lidar ładuje się. Pozostało: {(nextScanTime - Time.time):F1} s.");
            }
        }
    }

    private void UseLidar()
    {
        if (audioSource != null && scanSound != null)
        {
            audioSource.PlayOneShot(scanSound);
        }

        if (raycastOrigin == null) return;

        List<DotData> pendingDots = new List<DotData>();

        // Wyciągamy płaski wektor 'do przodu' gracza, ignorując patrzenie w podłogę/sufit
        Vector3 flatForward = raycastOrigin.forward;
        flatForward.y = 0;
        if (flatForward.sqrMagnitude < 0.001f) flatForward = raycastOrigin.up; // Zabezpieczenie przy patrzeniu idealnie w dół
        flatForward.Normalize();

        Quaternion baseRotation = Quaternion.LookRotation(flatForward);

        // Rozrzut bazowych promieni
        for (int i = 0; i < baseRays; i++)
        {
            float randomYaw = Random.Range(-horizontalScanAngle / 2f, horizontalScanAngle / 2f);
            float randomPitch = Random.Range(-verticalScanAngle / 2f, verticalScanAngle / 2f);

            Vector3 rayDirection = baseRotation * Quaternion.Euler(randomPitch, randomYaw, 0) * Vector3.forward;

            if (Physics.Raycast(raycastOrigin.position, rayDirection, out RaycastHit hit, scanDistance))
            {
                // Dodajemy główną kropkę z minimalnym odsunięciem od ściany
                pendingDots.Add(new DotData { 
                    position = hit.point + hit.normal * 0.01f, 
                    normal = hit.normal, 
                    distance = hit.distance 
                });

                // SYSTEM ADAPTACYJNEJ GĘSTOŚCI (Prawdziwe promienie pomocnicze)
                if (adaptiveDensity && hit.distance > 2.5f)
                {
                    // Im dalej, tym więcej promieni dodatkowych
                    int extraDots = Mathf.RoundToInt((hit.distance - 2f) * 1.5f);
                    extraDots = Mathf.Min(extraDots, 15); // Limit bezpieczeństwa

                    for (int e = 0; e < extraDots; e++)
                    {
                        // Zamiast płaskiej projekcji, odchylamy kąt oryginalnego promienia
                        float spreadAngle = adaptiveSpreadFactor * 20f; 
                        float randYaw = Random.Range(-spreadAngle, spreadAngle);
                        float randPitch = Random.Range(-spreadAngle, spreadAngle);

                        Vector3 extraRayDir = Quaternion.Euler(randPitch, randYaw, 0) * rayDirection;

                        // Wystrzeliwujemy PRAWDZIWY raycast - dzięki temu uniemożliwiamy artefakty w powietrzu
                        if (Physics.Raycast(raycastOrigin.position, extraRayDir, out RaycastHit extraHit, scanDistance))
                        {
                            pendingDots.Add(new DotData { 
                                position = extraHit.point + extraHit.normal * 0.01f, 
                                normal = extraHit.normal, 
                                distance = extraHit.distance + Random.Range(0.1f, 0.5f) // Lekkie opóźnienie
                            });
                        }
                    }
                }
            }
        }

        // Sortujemy kropki od najbliższej do najdalszej, by fala szła od gracza w przód
        pendingDots.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Uruchamiamy zoptymalizowaną falę
        StartCoroutine(SpawnDotsWave(pendingDots));
    }

    private IEnumerator SpawnDotsWave(List<DotData> dots)
    {
        float startTime = Time.time;
        int currentIndex = 0;

        while (currentIndex < dots.Count)
        {
            float currentWaveDistance = (Time.time - startTime) * waveSpeed;

            while (currentIndex < dots.Count && dots[currentIndex].distance <= currentWaveDistance)
            {
                SpawnRedDot(dots[currentIndex].position, dots[currentIndex].normal);
                currentIndex++;
            }

            yield return null; 
        }
    }

    private void SpawnRedDot(Vector3 position, Vector3 normal)
    {
        GameObject dot = null;

        if (dotPrefab != null)
        {
            dot = Instantiate(dotPrefab, position, Quaternion.LookRotation(normal));
        }
        else
        {
            dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dot.transform.position = position;
            dot.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f); 
            
            Destroy(dot.GetComponent<Collider>());

            Renderer renderer = dot.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = (dotMaterial != null) ? dotMaterial : generatedRedMaterial;
            }
        }

        if (dot != null)
        {
            Destroy(dot, dotLifetime);
        }
    }
}