using System;
using System.Collections.Generic;
using UnityEngine;
using GBuster;

public class StageController : MonoBehaviour 
{

    #region フィールド

    private GameManager gameManager;

    private Dictionary<Define.StageNum, StageData> stages;

    [SerializeField]
    private GameObject stagePrefab;

    [SerializeField]
    private GameObject tombstonePrefab;

    [SerializeField]
    private GameObject invisibleWallPrefab;

    [SerializeField]
    private GameObject invisibleCapsuleWallPrefab;

    private GameObject stageParent;
    private Transform wallParent;
    private Transform invisibleParent;
    private Transform enemyParent;

    #endregion


    #region プロパティ

    #endregion


    #region Unity メソッド

    /// <summary> 
    /// 初期化処理
    /// </summary>
    void Awake ()
	{
        stages = new Dictionary<Define.StageNum, StageData>();
        
        foreach (Define.StageNum stageNum in Enum.GetValues(typeof(Define.StageNum)))
        {
            StageData stageData = new StageData((byte)stageNum, (int)stageNum);
            stages.Add(stageNum, stageData);
        }
        
    }
	
	/// <summary> 
	/// 更新前処理
	/// </summary>
	void Start () 
	{
        gameManager = GameManager.Instance;
        MakeMap(Define.StageNum.Zero);
        gameManager.CurrentStage = stages[Define.StageNum.Zero];
	}
	
	/// <summary> 
	/// 更新処理
	/// </summary>
	void Update () 
	{
		
	}

    #endregion


    #region メソッド

    /// <summary>
    /// マップ内のオブジェクト生成
    /// </summary>
    /// <param name="stageNum">生成するマップのステージ番号</param>
    private void MakeMap(Define.StageNum stageNum)
    {
        // ステージ親オブジェクト作成・各階層のTransform取得
        stageParent = Instantiate(stagePrefab) as GameObject;
        wallParent = stageParent.transform.Find("Wall");
        invisibleParent = stageParent.transform.Find("Invisible");
        enemyParent = stageParent.transform.Find("Enemy");

        // 連なった壁の最初の座標と最後の座標のリスト
        List<int[,]> wallSequencePositions = new List<int[,]>();

        // 既にチェックした壁の座標リスト
        List<int[]> alreadyCheckedWidthWalls = new List<int[]>();
        List<int[]> alreadyCheckedHeightWalls = new List<int[]>();
        
        for (int z = 0; z < Define.mapSizeHeight; z++)
        {
            for (int x = 0; x < Define.mapSizeWidth; x++)
            {
                if (stages[stageNum].mapStatus[z, x] != StageData.MapType.Pole
                    && stages[stageNum].mapStatus[z, x] != StageData.MapType.Wall)
                {
                    continue;
                }

                //壁作成
                int[] wallPos = new int[2] { z, x };
                MakeWall(wallPos);

                StockWallSequenceFirstLastPos(stageNum, wallSequencePositions, wallPos, alreadyCheckedWidthWalls, true);
                StockWallSequenceFirstLastPos(stageNum, wallSequencePositions, wallPos, alreadyCheckedHeightWalls, false);
                
                if (Utility.IsContainedInPosList(wallPos, alreadyCheckedWidthWalls)
                    || Utility.IsContainedInPosList(wallPos, alreadyCheckedHeightWalls))
                    continue;

                // 縦横どちらにも連なっていない孤立した壁をリストに追加
                int[,] isolatedWallPositions = new int[,] { { wallPos[0], wallPos[1] }, { wallPos[0], wallPos[1] } };
                wallSequencePositions.Add(isolatedWallPositions);
            }
        }

        MakeInvisibleWalls(wallSequencePositions);
    }

    /// <summary>
    /// 壁生成
    /// </summary>
    /// <param name="wallPos">壁の座標</param>
    private void MakeWall(int[] wallPos)
    {
        Transform wallTransform = Instantiate(tombstonePrefab).GetComponent<Transform>();
        wallTransform.SetParent(wallParent);

        float[] pos = Utility.MapPosToWorldPos(wallPos);
        float posY = wallTransform.position.y;
        wallTransform.position = new Vector3(pos[1], posY, pos[0]);
    }

    /// <summary>
    /// 一方向に連なった壁の最初と最後の壁の位置をリストにストック
    /// </summary>
    /// <param name="stageNum">対象となるマップのステージ番号</param>
    /// <param name="wallSequencePositions">連なった壁の最初の座標と最後の座標のリスト</param>
    /// <param name="wallPosition">調べる壁の座標</param>
    /// <param name="alreadyCheckedWallPositions">既に調べた壁の座標リスト</param>
    /// <param name="isWidthCheck">横方向のチェックかどうか</param>
    private void StockWallSequenceFirstLastPos(Define.StageNum stageNum, List<int[,]> wallSequencePositions,
                                                int[] wallPosition, List<int[]> alreadyCheckedWallPositions, bool isWidthCheck)
    {
        // チェックする方向（縦横）を確認
        int dIndex = isWidthCheck ? 0 : 1;
        int[] dz = { 0, 1 };
        int[] dx = { 1, 0 };


        if (Utility.IsContainedInPosList(wallPosition, alreadyCheckedWallPositions))
            return;

        int wallPosZ = wallPosition[0];
        int wallPosX = wallPosition[1];
        int wallCount = 0;

        // wallCountなくしてもちゃんと不可視壁作成できるかチェック
        while (true)
        {
            wallPosZ += dz[dIndex];
            wallPosX += dx[dIndex];

            if (stages[stageNum].mapStatus[wallPosZ, wallPosX] != StageData.MapType.Pole
                && stages[stageNum].mapStatus[wallPosZ, wallPosX] != StageData.MapType.Wall)
            {
                wallPosZ -= dz[dIndex];
                wallPosX -= dx[dIndex];

                if (wallCount > 0)
                {
                    int[,] wallPositions = new int[2, 2] { { wallPosition[0], wallPosition[1] }, { wallPosZ, wallPosX } };
                    wallSequencePositions.Add(wallPositions);
                }
                wallCount = 0;
                break;
            }

            int[] nextWallPos = new int[2] { wallPosZ, wallPosX };
            alreadyCheckedWallPositions.Add(nextWallPos);
            wallCount++;
        }
    }


    /// <summary>
    /// 見えない壁生成
    /// </summary>
    /// <param name="wallPositions">見えない壁の位置</param>
    private void MakeInvisibleWalls(List<int[,]> wallPositions)
    {
        foreach (int[,] twoWallPos in wallPositions)
        {
            int[] wallPos0 = new int[2] { twoWallPos[0, 0], twoWallPos[0, 1] };
            int[] wallPos1 = new int[2] { twoWallPos[1, 0], twoWallPos[1, 1] };

            // 端と端の壁の距離
            float lenX = Mathf.Abs(wallPos0[1] - wallPos1[1]);
            float lenZ = Mathf.Abs(wallPos0[0] - wallPos1[0]);

            // 壁の初めと終わりにはカプセル状のものを配置
            Transform capsuleTransform0 = Instantiate(invisibleCapsuleWallPrefab).GetComponent<Transform>();
            capsuleTransform0.SetParent(invisibleParent);

            float[] worldPos = Utility.MapPosToWorldPos(wallPos0);
            float offset = 1f;
            capsuleTransform0.position = new Vector3(worldPos[1], capsuleTransform0.position.y, worldPos[0]);
            capsuleTransform0.GetComponent<CapsuleCollider>().radius = (Define.mapTileSize - offset) / 2f;

            // 連なりがない孤立した壁の場合ここで終了
            if (lenX == lenZ) continue;

            Transform capsuleTransform1 = Instantiate(invisibleCapsuleWallPrefab).GetComponent<Transform>();
            capsuleTransform1.SetParent(invisibleParent);

            worldPos = Utility.MapPosToWorldPos(wallPos1);
            capsuleTransform1.position = new Vector3(worldPos[1], capsuleTransform1.position.y, worldPos[0]);
            capsuleTransform1.GetComponent<CapsuleCollider>().radius = (Define.mapTileSize - offset) / 2f;
            
            // カプセル間の壁を配置
            Transform wallTransform = Instantiate(invisibleWallPrefab).GetComponent<Transform>();
            wallTransform.SetParent(invisibleParent);

            float[] invisibleWallPos = new float[2] { wallPos0[0] + lenZ / 2f, wallPos0[1] + lenX / 2f };
            worldPos = Utility.MapPosToWorldPos(invisibleWallPos);

            wallTransform.position = new Vector3(worldPos[1], wallTransform.position.y, worldPos[0]);
            float scaleX = lenX > 0 ? Define.mapTileSize * lenX : Define.mapTileSize - offset;
            float scaleZ = lenZ > 0 ? Define.mapTileSize * lenZ : Define.mapTileSize - offset;
            wallTransform.localScale = new Vector3(scaleX, wallTransform.localScale.y, scaleZ);
        }
    }

    #endregion

}
