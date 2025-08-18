using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private async UniTask MainGameLoad()
    {
        await ReadSpreadSheet.instance.LoadData();
        SceneManager.LoadScene("MainGame");
    }

    public void StartGame()
    {
        MainGameLoad();
    }

    public void StartSceneLoad()
    {
        SceneManager.LoadScene("StartScene");
    }
}