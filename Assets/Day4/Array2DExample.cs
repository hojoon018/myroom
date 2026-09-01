using UnityEngine;

public class Array2DExample : MonoBehaviour
{
    private void Start()
    {
        // 2차원 배열 (다차원 배열)
        // 게임 개발에서 2차원 배열은 주로 타일 기반 게임에서 사용된다.
        // 사용 예시 :
        // 3math 퍼즐게임, 플랫포머 맵, 인벤토리, 체스
        // 표 형태의 데이터를 표현하는 데 많이 사용

        int[,] enemyHealths = new int[2, 3];
        enemyHealths[0, 0] = 31;
        enemyHealths[0, 1] = 28;
        enemyHealths[0, 2] = 45;
        enemyHealths[1, 0] = 31;
        enemyHealths[1, 1] = 56;
        enemyHealths[1, 2] = 68;
        
        Debug.Log(enemyHealths[1, 1]);

        // 1. 배열은 행우선으로 접근을 해야한다. (캐시 히트를 위해서)
        for (int col = 0; col < enemyHealths.Length; col++)
        {
            for (int row = 0; row < enemyHealths.Length; row++)
            {
                Debug.Log($"enemyHealths[{col}][{row}]: {enemyHealths[col, row]}");
            }
        }
        
        // 2. 이렇게 행이 아닌 열우선으로 접근하면 (캐시 미스)
        for (int row = 0; row < enemyHealths.Length; row++)
        {
            for (int col = 0; col < enemyHealths.Length; col++)
            {
                Debug.Log($"enemyHealths[{col}][{row}]: {enemyHealths[col, row]}");
            }
        }

    }
    
}
