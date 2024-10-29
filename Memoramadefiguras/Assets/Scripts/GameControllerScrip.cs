using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public const int columns = 4;
    public const int rows = 2;
    public const float xspace = 4f;
    public const float yspace = -5f;

    public GameObject gameOverPanel; 
    public GameObject winPanel;
    [SerializeField] private MainImagesScript startObject;
    [SerializeField] private Sprite[] images;
    [SerializeField] private TextMeshProUGUI finalScoreText; 
    [SerializeField] private TextMeshProUGUI finalAttemptsText;

    private MainImagesScript firstOpen;
    private MainImagesScript secondOpen;
    private string sceneToLoad = "Menu";
    private string siguienteEscena = "SegundoNivel";

    private int score = 0;
    private int attempts = 0;
    private const int maxAttempts = 8;  // Máximo de intentos permitidos

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI attemptsText;

    private int[] Randomiser(int[] locations)
    {
        int[] array = (int[])locations.Clone();
        for (int i = 0; i < array.Length; i++)
        {
            int newArray = array[i];
            int j = Random.Range(i, array.Length);
            array[i] = array[j];
            array[j] = newArray;
        }
        return array;
    }

    private void Start()
    {
        if (startObject == null)
        {
            Debug.LogError("startObject no está asignado. Por favor, asigna un objeto en el Inspector.");
            return;
        }
        if (scoreText == null || attemptsText == null)
        {
            Debug.LogError("Por favor, asigna los objetos de texto de puntaje e intentos en el Inspector.");
            return;
        }
        if (images == null || images.Length == 0)
        {
            Debug.LogError("No se han asignado imágenes en el array 'images'. Por favor, asigna las imágenes en el Inspector.");
            return;
        }

        int[] locations = { 0, 0, 1, 1, 2, 2, 3, 3 };
        locations = Randomiser(locations);

        Vector3 startPosition = startObject.transform.position;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                MainImagesScript gameImage;
                if (i == 0 && j == 0)
                {
                    gameImage = startObject;
                }
                else
                {
                    gameImage = Instantiate(startObject) as MainImagesScript;
                }

                int index = j * columns + i;
                int id = locations[index];
                gameImage.ChangeSprite(id, images[id]);

                float positionX = (xspace * i) + startPosition.x;
                float positionY = (yspace * j) + startPosition.y;
                gameImage.transform.position = new Vector3(positionX, positionY, startPosition.z);

                Debug.Log($"Imagen creada en la posición: {positionX}, {positionY}");
            }
        }
    }

    public bool canOpen
    {
        get { return secondOpen == null; }
    }

    public void ImageOpen(MainImagesScript selectedImage)
    {
        if (firstOpen == null)
        {
            firstOpen = selectedImage;
        }
        else
        {
            secondOpen = selectedImage;
            StartCoroutine(CheckGuessed());
        }
    }

    private IEnumerator CheckGuessed()
    {
        if (firstOpen.SpriteId == secondOpen.SpriteId)
        {
            score++;
            scoreText.text = "Puntuacion: " + score;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            firstOpen.Close();
            secondOpen.Close();
        }

        firstOpen = null;
        secondOpen = null;

        attempts++;
        attemptsText.text = "Intentos: " + attempts;

        // Verifica si se han alcanzado los intentos máximos
        if(score >= 4)
        {
            WinPanel();
        }

        if (attempts >= maxAttempts)
        {
            ShowGameOver();
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

   public void ShowGameOver()
    {
        Time.timeScale = 0; // Pausa el juego
        gameOverPanel.SetActive(true); // Muestra el menú de Game Over
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void WinPanel()
    {
        Time.timeScale = 0;
        winPanel.SetActive(true);
        finalScoreText.text = "Puntuación: " + score;
        finalAttemptsText.text = "Intentos: " + attempts;
    }

    public void Siguiente()
    {
        SceneManager.LoadScene(siguienteEscena);
    }

    public void SalirDeLaApp()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
}
