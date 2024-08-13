using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class changeScene : MonoBehaviour
{
    public Button btn;
    public string NextSceneName = "";

    private void Start()
    {
        if (btn == null)
            btn = GetComponent<Button>();

        btn.onClick.AddListener(changeTheScene);
    }

    private void changeTheScene()
    {
        SceneManager.LoadScene(NextSceneName);
    }
}
