using UnityEngine;
using UnityEngine.UI; // Required for UI Image
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class LidarTool2D : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;

    [Header("UI Settings (Interfejs)")]
    [Tooltip("Parent container for the Lidar UI (Battery outline + fill bar)")]
    public GameObject lidarUIContainer;
    
    [Tooltip("The Image component of the fill bar (Image Type must be set to 'Filled')")]
    public Image cooldownBarFill;

    [Header("Sprite Settings (Animacja Ręki)")]
    [Tooltip("The Image component displaying the lidar hand on screen")]
    public Image lidarImage;
    [Tooltip("Sprite to show when the lidar is idle/waiting")]
    public Sprite idleSprite;
    [Tooltip("Sprite to show when the lidar is actively scanning")]
    public Sprite activeSprite;
    [Tooltip("How long the active sprite is shown (in seconds)")]
    public float activeSpriteDuration = 2f;

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
    [Tooltip("Liczba promieni w poziomie (szerokość) - Zmniejszono dla rzadszego efektu")]
    public int raysHorizontal = 30;
    
    [Tooltip("Liczba promieni w pionie (wysokość) - Zmniejszono dla rzadszego efektu")]
    public int raysVertical = 15;

    [Tooltip("Stopień chaosu bazowej siatki (0 = od linijki, 1 = mocny rozrzut)")]
    [Range(0f, 1f)]
    public float baseScatter = 0.5f;
    
    [Tooltip("WŁĄCZONE: System automatycznie dodaje więcej kropek na dalekich obiektach, aby utrzymać równomierne pokrycie ścian.")]
    public bool adaptiveDensity = true;
    
    [Tooltip("Jak szeroko rozsypują się dodatkowe promienie. Im mniejsza wartość, tym gęstsze klastry.")]
    public float adaptiveSpreadFactor = 0.15f;

    [Header("Wave Effect Settings (Efekt Fali)")]
    [Tooltip("How fast the wave travels (m/s). Lower is slower.")]
    public float waveSpeed = 25f;

    [Header("Visuals (Wygląd)")]
    [Tooltip("Gradient kolorów zależny od odległości. Domyślnie: Czerwony (blisko) -> Niebieski (daleko)")]
    public Gradient dotColorGradient;

    [Tooltip("Prefab for the red dot. If empty, a primitive sphere will be created dynamically.")]
    public GameObject dotPrefab;
    
    [Tooltip("Material for the primitive dots. Leave empty to use a generated default material.")]
    public Material dotMaterial;
    
    [Tooltip("How long (in seconds) the dots stay visible before disappearing")]
    public float dotLifetime = 7f;

    [Tooltip("Sound to play when scanning")]
    public AudioClip scanSound;

    // Timer
    private float nextScanTime = 0f;
    private Material generatedMaterial;
    
    // Optymalizacja zmiany kolorów setek obiektów naraz
    private MaterialPropertyBlock propBlock;
    
    // Przechowuje referencję do korutyny zmieniającej sprite, by móc ją przerwać w razie zmiany slotu
    private Coroutine spriteSwapCoroutine;

    private struct DotData
    {
        public Vector3 position;
        public Vector3 normal;
        public float distance;
    }

    private void Reset()
    {
        ResetGradientToThermal();
    }

    [ContextMenu("Ustaw gradient na Termowizje")]
    public void ResetGradientToThermal()
    {
        dotColorGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[4];
        colorKeys[0] = new GradientColorKey(Color.red, 0.0f);       
        colorKeys[1] = new GradientColorKey(Color.yellow, 0.25f);   
        colorKeys[2] = new GradientColorKey(Color.green, 0.5f);     
        colorKeys[3] = new GradientColorKey(Color.blue, 1.0f);      

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

        dotColorGradient.SetKeys(colorKeys, alphaKeys);
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        propBlock = new MaterialPropertyBlock();

        if (raycastOrigin == null && Camera.main != null)
        {
            raycastOrigin = Camera.main.transform;
        }

        if (dotColorGradient == null || dotColorGradient.colorKeys.Length == 0)
        {
            ResetGradientToThermal();
        }

        if (dotMaterial == null)
        {
            GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            generatedMaterial = new Material(tempSphere.GetComponent<Renderer>().sharedMaterial);
            Destroy(tempSphere);
            
            generatedMaterial.EnableKeyword("_EMISSION");
        }

        // Hide UI by default when the game starts
        ShowLidarUI(false);
    }

    public void SetToolActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        
        if (isActive) 
        {
            Debug.Log("Informacja: Lidar został aktywowany.");
            
            // Show the Lidar UI when equipped
            ShowLidarUI(true);

            // Reset to idle sprite when equipped
            if (lidarImage != null && idleSprite != null) 
            {
                lidarImage.sprite = idleSprite;
            }
        }
        else
        {
            // Stop the sprite swap coroutine if the player puts the lidar away while scanning
            if (spriteSwapCoroutine != null) 
            {
                StopCoroutine(spriteSwapCoroutine);
                spriteSwapCoroutine = null;
            }

            // Hide the Lidar UI when put away
            ShowLidarUI(false);

            // Reset to idle sprite when put away
            if (lidarImage != null && idleSprite != null) 
            {
                lidarImage.sprite = idleSprite;
            }
        }
    }

    void Update()
    {
        // Smoothly update the fill bar UI every frame
        UpdateCooldownUI();

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
        
        // Handle Sprite Swap Animation
        if (lidarImage != null && activeSprite != null)
        {
            if (spriteSwapCoroutine != null) StopCoroutine(spriteSwapCoroutine);
            spriteSwapCoroutine = StartCoroutine(SwapSpriteTemporarily());
        }

        if (raycastOrigin == null) return;

        List<DotData> pendingDots = new List<DotData>();

        Vector3 flatForward = raycastOrigin.forward;
        flatForward.y = 0;
        if (flatForward.sqrMagnitude < 0.001f) flatForward = raycastOrigin.up; 
        flatForward.Normalize();

        Quaternion baseRotation = Quaternion.LookRotation(flatForward);

        // Obliczanie początkowych wartości siatki
        float startYaw = -horizontalScanAngle / 2f;
        float startPitch = -verticalScanAngle / 2f;
        
        float yawStep = horizontalScanAngle / Mathf.Max(1, raysHorizontal - 1);
        float pitchStep = verticalScanAngle / Mathf.Max(1, raysVertical - 1);

        // 1. ZOPTYMALIZOWANA SIATKA Z SZUMEM DLA RÓWNOMIERNEGO POKRYCIA
        for (int x = 0; x < raysHorizontal; x++)
        {
            for (int y = 0; y < raysVertical; y++)
            {
                float baseYaw = startYaw + (x * yawStep);
                float basePitch = startPitch + (y * pitchStep);

                // Dodajemy lekki, kontrolowany szum aby zniwelować "efekt lasera od linijki"
                float jitterYaw = Random.Range(-yawStep * 0.5f * baseScatter, yawStep * 0.5f * baseScatter);
                float jitterPitch = Random.Range(-pitchStep * 0.5f * baseScatter, pitchStep * 0.5f * baseScatter);

                float finalYaw = baseYaw + jitterYaw;
                float finalPitch = basePitch + jitterPitch;

                Vector3 rayDirection = baseRotation * Quaternion.Euler(finalPitch, finalYaw, 0) * Vector3.forward;

                if (Physics.Raycast(raycastOrigin.position, rayDirection, out RaycastHit hit, scanDistance))
                {
                    pendingDots.Add(new DotData { 
                        position = hit.point + hit.normal * 0.01f, 
                        normal = hit.normal, 
                        distance = hit.distance 
                    });

                    // 2. SYSTEM ADAPTACYJNY OPARTY NA ZŁOTEJ SPIRALI (Idealny rozkład kołowy)
                    if (adaptiveDensity && hit.distance > 0.5f)
                    {
                        // ZMNIEJSZONO MNOŻNIK: Kropki w oddali będą generować się rzadziej (z 2.5f na 1.5f)
                        int extraDots = Mathf.RoundToInt(hit.distance * 1.5f);
                        extraDots = Mathf.Min(extraDots, 10); // Obniżono maksymalny limit z 15 do 10

                        // 137.5 stopni w radianach to idealny kąt ciagu Fibonacciego do modelowania np. ziaren słonecznika
                        float goldenAngle = 2.399963f; 

                        for (int e = 0; e < extraDots; e++)
                        {
                            // Obliczanie idealnego punktu na kole (spirali) bez nakładania się kropek na siebie
                            float radius = Mathf.Sqrt((e + 0.5f) / extraDots) * (adaptiveSpreadFactor * 20f);
                            float theta = e * goldenAngle;

                            float offsetYaw = radius * Mathf.Cos(theta);
                            float offsetPitch = radius * Mathf.Sin(theta);

                            Vector3 extraRayDir = baseRotation * Quaternion.Euler(finalPitch + offsetPitch, finalYaw + offsetYaw, 0) * Vector3.forward;

                            if (Physics.Raycast(raycastOrigin.position, extraRayDir, out RaycastHit extraHit, scanDistance))
                            {
                                pendingDots.Add(new DotData { 
                                    position = extraHit.point + extraHit.normal * 0.01f, 
                                    normal = extraHit.normal, 
                                    distance = extraHit.distance + Random.Range(0.05f, 0.2f) 
                                });
                            }
                        }
                    }
                }
            }
        }

        pendingDots.Sort((a, b) => a.distance.CompareTo(b.distance));
        StartCoroutine(SpawnDotsWave(pendingDots));
    }

    private IEnumerator SwapSpriteTemporarily()
    {
        // Change to active scanning sprite
        lidarImage.sprite = activeSprite;
        
        // Wait for the specified duration (2 seconds)
        yield return new WaitForSeconds(activeSpriteDuration);
        
        // Revert back to idle sprite
        if (idleSprite != null) 
        {
            lidarImage.sprite = idleSprite;
        }
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
                SpawnRedDot(dots[currentIndex].position, dots[currentIndex].normal, dots[currentIndex].distance);
                currentIndex++;
            }

            yield return null; 
        }
    }

    private void SpawnRedDot(Vector3 position, Vector3 normal, float distance)
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
                renderer.sharedMaterial = (dotMaterial != null) ? dotMaterial : generatedMaterial;

                float normalizedDistance = Mathf.Clamp01(distance / scanDistance);
                Color dotColor = dotColorGradient.Evaluate(normalizedDistance);

                renderer.GetPropertyBlock(propBlock);
                
                propBlock.SetColor("_Color", dotColor);      
                propBlock.SetColor("_BaseColor", dotColor);  
                propBlock.SetColor("_EmissionColor", dotColor * 2f); 
                
                renderer.SetPropertyBlock(propBlock);
            }
        }

        if (dot != null)
        {
            Destroy(dot, dotLifetime);
        }
    }

    // Helper method to toggle Lidar UI visibility
    private void ShowLidarUI(bool show)
    {
        // Wyłączamy/włączamy główny kontener
        if (lidarUIContainer != null)
        {
            lidarUIContainer.SetActive(show);
        }

        // ZABEZPIECZENIE: Zawsze ręcznie wyłączamy również sam pasek, 
        // na wypadek gdyby w hierarchii Unity nie był on dzieckiem głównego kontenera
        if (cooldownBarFill != null)
        {
            cooldownBarFill.gameObject.SetActive(show);
        }
    }

    // Calculates and updates the fill amount of the cooldown bar
    private void UpdateCooldownUI()
    {
        if (cooldownBarFill != null && cooldownBarFill.gameObject.activeInHierarchy)
        {
            // How much time is left until we can scan again? (Clamped between 0 and cooldown duration)
            float remainingTime = Mathf.Max(0f, nextScanTime - Time.time);
            
            // Calculate fill percentage (0.0 when just used, 1.0 when fully ready)
            float fillPercentage = 1f - (remainingTime / scanCooldown);
            
            cooldownBarFill.fillAmount = fillPercentage;
        }
    }
}