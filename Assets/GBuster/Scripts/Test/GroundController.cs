using UnityEngine;
using GBuster;

public class GroundController : MonoBehaviour {

    #region フィールド

    private Transform _transform;

    #endregion

    /// <summary> 
    /// 初期化処理
    /// </summary>
    void Awake ()
    {
        _transform = transform;

        // 必要があればプロパティに移動
        float scaleX = Define.mapSizeWidth * Define.mapTileSize;
        float scaleZ = Define.mapSizeHeight * Define.mapTileSize;
        float posX = scaleX / 2;
        float posZ = scaleZ / 2;

        _transform.localScale = new Vector3(scaleX, _transform.localScale.y, scaleZ);
        _transform.position = new Vector3(posX, _transform.position.y, posZ);
    }

}
