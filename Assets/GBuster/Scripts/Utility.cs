using System.Collections.Generic;

namespace GBuster
{
    public class Utility
    {

        #region メソッド

        /// <summary>
        /// positionがpositionListに入っているかどうか
        /// </summary>
        /// <param name="pos">対象の座標</param>
        /// <param name="positions">座標リスト</param>
        /// <returns>リストに入っているかどうかの真偽値</returns>
        public static bool IsContainedInPosList(int[] pos, List<int[]> positions)
        {
            foreach (int[] p in positions)
            {
                if (p[0] == pos[0] && p[1] == pos[1])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// int型のマップ座標からワールド座標への変換
        /// </summary>
        /// <param name="mapPos">マップ座標</param>
        /// <returns>ワールド座標</returns>
        public static float[] MapPosToWorldPos(int[] mapPos)
        {
            float worldPosZ = (0.5f + mapPos[0]) * Define.mapTileSize;
            float worldPosX = (0.5f + mapPos[1]) * Define.mapTileSize;
            return new float[2] { worldPosZ, worldPosX };
        }

        /// <summary>
        /// float型のマップ座標からワールド座標への変換
        /// </summary>
        /// <param name="mapPos"></param>
        /// <returns></returns>
        public static float[] MapPosToWorldPos(float[] mapPos)
        {
            float worldPosZ = (0.5f + mapPos[0]) * Define.mapTileSize;
            float worldPosX = (0.5f + mapPos[1]) * Define.mapTileSize;
            return new float[2] { worldPosZ, worldPosX };
        }

        // まだガワだけ
        /// <summary>
        /// float型のワールド座標からマップ座標への変換
        /// </summary>
        /// <param name="worldPos">ワールド座標</param>
        /// <returns>マップ座標</returns>
        public static int[] WorldPosToMapPos(float[] worldPos)
        {
            int posZ = 0;
            int posX = 0;
            return new int[2] { posZ, posX };
        }

        #endregion

    }
}
