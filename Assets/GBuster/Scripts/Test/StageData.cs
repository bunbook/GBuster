using System.Collections.Generic;
using UnityEngine;
using GBuster;

public class StageData
{
    
    #region 列挙型

    public enum MapType
    {
        Blank = 0,
        Start,
        Goal,
        Pole,
        Wall
    }

    #endregion


    #region フィールド

    public MapType[,] mapStatus;

    public int[] startPosition;
    public int[] goalPosition;

    #endregion


    #region コンストラクタ

    public StageData()
    {
        Init(Random.Range(byte.MinValue, byte.MaxValue + 1), Random.Range(0, 2));
    }

    public StageData(byte randamSeed, int stageNum)
    {
        Init((int)randamSeed, stageNum);
    }

    #endregion
    

    #region メソッド

    /// <summary>
    /// ステージデータ初期化
    /// </summary>
    /// <param name="randamSeed">疑似乱数の種</param>
    /// <param name="stageNum">初期化するステージの番号</param>
    private void Init(int randamSeed, int stageNum)
    {
        // mapは番兵のために(縦サイズ + 1) * (横サイズ + 1)の大きさで確保
        mapStatus = new MapType[Define.mapSizeHeight + 1, Define.mapSizeWidth + 1];

        // ステージ番号の奇数偶数でスタートとゴールの位置を変更
        int[] startGoalPosXs = new int[2] { 1, Define.mapSizeWidth - 2 };
        startPosition = new int[2] { 0, startGoalPosXs[stageNum % 2] };
        goalPosition = new int[2] { Define.mapSizeHeight - 1, startGoalPosXs[1 - stageNum % 2] };

        InitMapStatus();
        SetMapStatusByDruagasAlgorithm(randamSeed);
        ReducePoleInMapStatus();
    }

    /// <summary>
    /// mapのベース部分を初期化
    /// </summary>
    private void InitMapStatus()
    {
        // 全体を空白で初期化
        for (int z = 0; z < Define.mapSizeHeight + 1; z++)
        {
            for (int x = 0; x < Define.mapSizeWidth + 1; x++)
            {
                mapStatus[z, x] = MapType.Blank;
            }
        }

        // 4方向いい感じに走査できるやつ
        int[] dx = new int[4] { 1, 0, -1, 0 };
        int[] dz = new int[4] { 0, 1, 0, -1 };

        int wallPosX = 0;
        int wallPosZ = 0;

        // 周囲4辺に外壁を配置
        // 下->右->上->左 の順
        for (int wallNo = 0; wallNo < 4; wallNo++)
        {
            int maxWallCount = (wallNo % 2 == 0 ? Define.mapSizeWidth : Define.mapSizeHeight) - 1;

            for (int wallCount = 0; wallCount < maxWallCount; wallCount++)
            {
                mapStatus[wallPosZ, wallPosX] = MapType.Wall;
                wallPosX += dx[wallNo];
                wallPosZ += dz[wallNo];
            }
        }

        // スタート・ゴール配置
        mapStatus[startPosition[0], startPosition[1]] = MapType.Start;
        mapStatus[goalPosition[0], goalPosition[1]] = MapType.Goal;
    }

    /// <summary>
    /// ドルアーガの塔におけるダンジョン生成アルゴリズムを用いたmap内部の自動生成
    /// </summary>
    /// <param name="randamSeed">疑似乱数の種</param>
    private void SetMapStatusByDruagasAlgorithm(int randamSeed)
    {
        // 偶数座標の空白部に柱設置
        List<int[]> polePositions = new List<int[]>();
        for (int z = 2; z < Define.mapSizeHeight - 2; z += 2)
        {
            for (int x = 2; x < Define.mapSizeWidth - 2; x += 2)
            {
                if (mapStatus[z, x] != MapType.Blank)
                    continue;

                mapStatus[z, x] = MapType.Pole;
                int[] polePos = new int[2] { z, x };
                polePositions.Add(polePos);
            }
        }
        // return;

        // 内部の壁生成
        // ドルアーガの塔の迷路生成アルゴリズム
        int[,] processedPolePositionsMap = new int[Define.mapSizeHeight + 1, Define.mapSizeWidth + 1];
        List<int[]> wallPositions = new List<int[]>();

        // XORするビット番号列
        int[] tapSequence = { 4, 7 };

        // 随時更新される疑似乱数の種
        int seed = randamSeed;

        // 4方向いい感じに走査できるやつ
        int[] dx = new int[4] { 0, 0, 1, -1 };
        int[] dz = new int[4] { 1, -1, 0, 0 };

        // 柱ごとに処理
        foreach (int[] polePos in polePositions)
        {
            // 処理済みの柱なら飛ばして次の柱へ
            if (processedPolePositionsMap[polePos[0], polePos[1]] >= 1)
                continue;

            // チェックする座標
            int checkingPosX = polePos[1];
            int checkingPosZ = polePos[0];

            // 現在処理している柱の座標
            int[] currentPolePos = new int[2] { polePos[0], polePos[1] };

            // int breakCount = 0;
            while (true)
            {
                // 疑似乱数生成
                // 0:上, 1:下, 2:右, 3左:
                int preRandNum = GeneratePresudoRandomNumByILFSR(tapSequence, ref seed);

                // チェック座標を疑似乱数の示す方向にずらす
                checkingPosX += dx[preRandNum];
                checkingPosZ += dz[preRandNum];

                // チェック座標が空白なら壁設置
                if (mapStatus[checkingPosZ, checkingPosX] == MapType.Blank)
                {
                    mapStatus[checkingPosZ, checkingPosX] = MapType.Wall;
                }
                else if(mapStatus[checkingPosZ, checkingPosX] == MapType.Wall)
                {
                    // Debug.Log("(" + z + ", " + x + ")" + "ダブった\n");
                    // チェック座標の逆方向を見て空白なら壁配置
                    checkingPosX = currentPolePos[1] - dx[preRandNum];
                    checkingPosZ = currentPolePos[0] - dz[preRandNum];
                    // checkingPosX += -2 * dx[preRandNum];
                    // checkingPosZ += -2 * dz[preRandNum];

                    if (mapStatus[checkingPosZ, checkingPosX] == MapType.Blank)
                        mapStatus[checkingPosZ, checkingPosX] = MapType.Wall;
                    
                }
                // 柱を処理済みに
                processedPolePositionsMap[currentPolePos[0], currentPolePos[1]] += 1;

                // チェック座標を同方向の隣にずらす
                checkingPosX += dx[preRandNum];
                checkingPosZ += dz[preRandNum];

                // 柱でなければbreakして次の柱へ
                if (mapStatus[checkingPosZ, checkingPosX] != MapType.Pole)
                    break;
                
                //処理済みの柱ならbreakして次の柱へ
                currentPolePos = new int[2] { checkingPosZ, checkingPosX };
                if (processedPolePositionsMap[currentPolePos[0], currentPolePos[1]] >= 1)
                    break;

                // デバッグ用無限ループ防止
                // breakCount++;
                // if (breakCount > 100)
                //     break;
            }
        }
        // Debug.Log(GetMapStatusString());
    }

    /// <summary>
    /// 反転線形帰還シフトレジスタによる疑似乱数生成器
    /// </summary>
    /// <param name="tapSequence">XORするビット番号列</param>
    /// <param name="seed">疑似乱数の種</param>
    /// <returns>疑似乱数（0-3）</returns>
    private int GeneratePresudoRandomNumByILFSR(int[] tapSequence, ref int seed)
    {
        int currentBit = 0;
        int nextBit = 0;

        currentBit = seed & 1;
        seed = ILFSR(tapSequence, seed);
        nextBit = seed & 1;
        return (nextBit << 1) | currentBit;
    }

    /// <summary>
    /// 反転線形帰還シフトレジスタ
    /// </summary>
    /// <param name="tapSequence">XORするビット番号列</param>
    /// <param name="seed">初期ビット列</param>
    /// <returns>1ビット</returns>
    private int ILFSR(int[] tapSequence, int seed)
    {
        int bit = 0;
        foreach (int tap in tapSequence)
        {
            bit ^= (seed >> tap) & 1;
        }
        return ((seed << 1) | (bit ^ 1));
    }

    /// <summary>
    /// 柱削減（三方が空白の柱を空白に）
    /// </summary>
    private void ReducePoleInMapStatus()
    {
        int[] dx = new int[4] { 0, 0, -1, 1 };
        int[] dz = new int[4] { 1, -1, 0, 0 };

        for (int z = 2; z < Define.mapSizeHeight - 1; z += 2)
        {
            for (int x = 2; x < Define.mapSizeWidth - 1; x += 2)
            {
                int linkedBlanklNum = 0;
                for (int d = 0; d < 4; d++)
                {
                    if (mapStatus[z + dz[d], x + dx[d]] == MapType.Blank)
                        linkedBlanklNum++;
                }
                if (linkedBlanklNum == 3)
                    mapStatus[z, x] = MapType.Blank;
            }
        }
    }

    /// <summary>
    /// mapStatusの文字列化
    /// </summary>
    /// <returns>mapStatusの文字列</returns>
    public string GetMapStatusString()
    {
        string str = string.Empty;
        for (int z = 0; z < Define.mapSizeHeight + 1; z++)
        {
            for (int x = 0; x < Define.mapSizeWidth + 1; x++)
            {
                switch (mapStatus[z, x])    
                {
                    case MapType.Blank:
                        str += "_";
                        break;
                    case MapType.Start:
                        str += "s";
                        break;
                    case MapType.Goal:
                        str += "g";
                        break;
                    case MapType.Pole:
                        str += "p";
                        break;
                    case MapType.Wall:
                        str += "w";
                        break;
                }
                str += " ";
            }
            str += "\n";
        }
        return str;
    }

    #endregion
}
