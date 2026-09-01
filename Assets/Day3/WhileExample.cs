using UnityEngine;

public class WhileExample : MonoBehaviour
{
    private void Start()
    {
        // while 반복문은 조건식이 참일 동안만 반복 실행되는 반복문
        // for 반복문과 다르게 괄호 내부에 조건식만 들어간다.
        
        /*
         while (조건식)
         {
            조건식이 참일 동안만 반복할 코드들
         }
         */

        int i = 0;

        while (true)
        {
            Debug.Log("안녕하세요.");

            i++;  // 증감식을 빼먹으면 반복문이 무한히 실행되는 무한 루프에 빠져서 유니티가 멈춘다.

            if (i > 1000)
            {
                break;  // 반복문을 탈출 하는 키워드
            }
        }
        
        // for : 반복하고자 하는 순서가 정해져 있거나 조건이 명확할 때
        // while : 반복하고자 하는 순서가 정해져 있지 않을 때나 조건이 유동적일 경우
        
    }
    
}
