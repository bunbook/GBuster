using UnityEngine;

/// <summary>
/// ジェネリックなシングルトンクラス
/// （参考）http://naichilab.blogspot.jp/2013/11/unitymanager.html
/// </summary>
/// <typeparam name="T">Monobehaviourを継承している型</typeparam>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{

    #region フィールド

    private static T instance;

	#endregion


	#region public プロパティ

    public static T Instance
    {
        get
        {
            instance = instance ?? (T)FindObjectOfType(typeof(T));

            if (instance == null)
                Debug.LogError(typeof(T) + "is nothing");
            
            return instance;
        }
    }

    #endregion


    #region protected メソッド

    /// <summary>
    /// シングルトンの初期設定
    /// </summary>
    protected void InitSingleton()
    {
        bool isRootObject = !this.transform.parent;

        if (isRootObject)
        {
            if (this != Instance)
                Destroy(this.gameObject);
            else
                DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (this != Instance)
                Destroy(this);
        }
    }

    #endregion

    /// <summary>
    /// 使用例
    /// </summary>
    /*
    using UnityEngine;
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public void Awake()
        {
            InitSingleton();
        }
    }
    */

}
