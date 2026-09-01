using System;
using UnityEngine;

public class ArrayExample : MonoBehaviour
{
    private void Start()
    {
        
        // 배열 : 같은 자료형의 데이터를 한 그룹으로 묶어서 사용하는 것
        // -> 연관된 데이터들을 모아서 관리하기 위해 사용한다.

        int monsterHealth1 = 31;
        int monsterHealth2 = 28;
        int monsterHealth3 = 45;
        int monsterHealth4 = 52;
        
        // 배열(변수) 선언
        // 자료형[] 배열명 = new 자료형 [크기(개수)]
        // int[] monsterHealths = new int[5] { 31, 28, 45, 52, 66 };
        // int[] monsterHealths = new int[] { 31, 28, 45, 52, 66 };
        int[] monsterHealths = { 31, 28, 45, 52, 66 };
        // -> int 자료형 50개를 담을 수 있는 연속된 메모리 공간이 할당된다. (4바이트 * 50)
        // 데이터 공간 하나하나를 보통 "Item"이라고 부른다.
        // 그리고 이 아이템에 접근할 수 있는 랜덤 액세스를 제공하는데 이 순서를 "인덱스"라고 한다.

        monsterHealths[0] = 31;
        monsterHealths[1] = 28;
        monsterHealths[2] = 45;
        monsterHealths[3] = 52;
        monsterHealths[4] = 66;

        for (int i = 0; i < monsterHealths.Length; i++)
        {
            Debug.Log($"몬스터 {i}의 체력: {monsterHealths[i]}");
        }
        
        
        // 배열의 속성과 기능
        int[] ages = new int[4] { 30, 32, 27, 25 };
        
        Array.Sort(ages);       // 오름차순
        Array.Reverse(ages);    // 내림차순
        
        foreach (int age in ages)
        {
            Debug.Log(age);     // 정렬이 되었는지 확인
        }

    }
    
}
