using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    idle,
    playing,
    endGame
}
public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject cam;

    bool exitGame;
    bool restartGame = false;

    private void Awake()
    {

        Invoke(nameof(BugFixFunc), 0.05f);
    }

    void BugFixFunc()
    {
        restartGame = false;
    }

    void OnGUI()
    {
        ExitGame(); // Display exit game GUI
        Restart(); // Display restart game GUI
    }

    void ExitGame()
    {
        GUI.backgroundColor = Color.red;
        if (GUI.Button(new Rect(Screen.width - 60, 10, 50, 50), "X")) // If the X button is pressed
        {
            exitGame = true;
        }
        if (exitGame) // If exitGame is true then display the GUI below
        {
            GUI.Box(new Rect(Screen.width / 2 - 120, Screen.height / 2 + 100, 240, 80), "Exit game?");
            GUI.backgroundColor = Color.red;
            if (GUI.Button(new Rect(Screen.width / 2 - 105, Screen.height / 2 + 140, 100, 25), "Yes")) // If YES is pressed
            {
                Application.Quit(); // Quit the game
            }
            GUI.backgroundColor = Color.green;
            if (GUI.Button(new Rect(Screen.width / 2 + 5, Screen.height / 2 + 140, 100, 25), "No")) // if NO is pressed
            {
                exitGame = false;
            }
        }
    }

    void Restart()
    {
        if (UIMain.curLife <= 0) restartGame = true;



        GUI.backgroundColor = Color.green;
        if (GUI.Button(new Rect(10, 10, 50, 50), "Reset")) // If the Reset button is pressed
        {
            restartGame = true;
        }
        if (restartGame) // If restartGame is true then display the GUI below
        {
            GUI.Box(new Rect(Screen.width / 2 - 120, Screen.height / 2 + 100, 240, 80), "Restart game?");
            GUI.backgroundColor = Color.blue;
            if (GUI.Button(new Rect(Screen.width / 2 - 105, Screen.height / 2 + 140, 100, 25), "Yes")) // If YES is pressed
            {               
                RestartGame();
            }
            GUI.backgroundColor = Color.green;
            if (GUI.Button(new Rect(Screen.width / 2 + 5, Screen.height / 2 + 140, 100, 25), "No")) // if NO is pressed
            {
                restartGame = false;
            }
        }
    }

    public void RestartGame()
    {
        MovementRealize.canMove = true;//возвращает возможность перемещения
        UIMain.refresh = true;//статическая переменная, которая триггерит нестатическую функцию сброса переменных      
        SceneManager.LoadScene("mainsquare");//загружает сцену заново
    }
}

