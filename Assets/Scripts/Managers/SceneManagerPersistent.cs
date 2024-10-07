using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif


public enum SceneTypes
{
    MainMenu,
    LevelSelect,
    Level
}


[System.Serializable]
public class SceneField
{
    [SerializeField]
    private Object m_SceneAsset;

    [SerializeField]
    private string m_SceneName = "";
    public string SceneName
    {
        get { return m_SceneName; }
    }

    // makes it work with the existing Unity methods (LoadLevel/LoadScene)
    public static implicit operator string( SceneField sceneField )
    {
        return sceneField.SceneName;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer 
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginProperty(_position, GUIContent.none, _property);
        SerializedProperty sceneAsset = _property.FindPropertyRelative("m_SceneAsset");
        SerializedProperty sceneName = _property.FindPropertyRelative("m_SceneName");
        _position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);
        if (sceneAsset != null)
        {
            sceneAsset.objectReferenceValue = EditorGUI.ObjectField(_position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false); 

            if( sceneAsset.objectReferenceValue != null )
            {
                sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
            }
        }
        EditorGUI.EndProperty( );
    }
}
#endif
public class SceneManagerPersistent : MonoBehaviour
{
    private static SceneManagerPersistent _instance;
    public static SceneManagerPersistent Instance
    {
        get
        {
            if (!_instance)
            {
                Debug.LogError("SceneMangerPersistent is null");
            }
            return _instance;
        }
    }
    
    [SerializeField] private SceneField mainMenu;
    [SerializeField] private SceneField levelSelect;
    [SerializeField] private SceneField loadingScene;
    [SerializeField] private SceneField[] levels;
    
    private AsyncOperation _asyncOperation;

    public string NextScene { get; private set; }
    public LoadSceneMode LoadSceneMode { get; private set; }

    private void Awake()
    {
        List<SceneManagerPersistent> soundManagers = FindObjectsOfType<SceneManagerPersistent>().ToList();
        if (soundManagers.Count > 1)
        {
            foreach (SceneManagerPersistent soundManager in soundManagers)
            {
                if (soundManager != _instance)
                {
                    Destroy(soundManager.gameObject);
                }
            }
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void LoadNextScene(SceneTypes sceneType, LoadSceneMode loadSceneMode, bool useLoadingScene, int index = 0)
    {
        if (_asyncOperation != null && !_asyncOperation.isDone) return;
        string sceneName;
        switch (sceneType)
        {
            case SceneTypes.MainMenu:
                sceneName = mainMenu;
                break;
            case SceneTypes.LevelSelect:
                sceneName = levelSelect;
                break;
            case SceneTypes.Level:
                sceneName = levels[index];
                break;
            default:
                sceneName = mainMenu;
                break;
        }
        Debug.Log(sceneName);
        NextScene = sceneName;
        LoadSceneMode = loadSceneMode;
        if (useLoadingScene)
        {
            SceneManager.LoadScene(loadingScene);
        }
        else
        {
            StartCoroutine(LoadSceneAsync());
        }
    }
    
    private IEnumerator LoadSceneAsync()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        _asyncOperation = SceneManager.LoadSceneAsync(NextScene, LoadSceneMode);
        _asyncOperation.allowSceneActivation = false;
        while (_asyncOperation.progress < 0.9f)
        {
            yield return null;
        }
        _asyncOperation.allowSceneActivation = true;
        if (LoadSceneMode == LoadSceneMode.Additive)
        {
            SceneManager.UnloadSceneAsync(thisScene);
        }
        yield return null;
    }
    
}
