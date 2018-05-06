using System.Collections.Generic;

namespace GBuster
{
    public class Utility
    {
        
        #region メソッド

        // positionがpositionListに入っているかどうか
        public static bool IsContainedInPosList(int[] pos, List<int[]> positions)
        {
            foreach (int[] p in positions)
            {
                if (p[0] == pos[0] && p[1] == pos[1])
                    return true;
            }
            return false;
        }

        public static float[] MapPosToWorldPos(int[] mapPos)
        {
            float worldPosZ = (0.5f + mapPos[0]) * Define.mapTileSize;
            float worldPosX = (0.5f + mapPos[1]) * Define.mapTileSize;
            return new float[2] { worldPosZ, worldPosX };
        }

        public static float[] MapPosToWorldPos(float[] mapPos)
        {
            float worldPosZ = (0.5f + mapPos[0]) * Define.mapTileSize;
            float worldPosX = (0.5f + mapPos[1]) * Define.mapTileSize;
            return new float[2] { worldPosZ, worldPosX };
        }

        // まだガワだけ
        public static int[] WorldPosToMapPos(float z, float x)
        {
            int posZ = 0;
            int posX = 0;
            return new int[2] { posZ, posX };
        }

        #endregion

    }
}
