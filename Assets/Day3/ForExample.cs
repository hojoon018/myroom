using UnityEngine;

// 반복문 : 설정한 조건이 True일 동안(거짓이 될 때까지) 코드 블록 내부를 반복해서 실행한다.
        
// for 반복문
        
/*
 for (초기식, 조건식, 증감연산자)
 {
    반복할 코드들
 }
 // 초기식 : 처음 한 번 실행할 식(보통 변수의 선언) : ex. int i = 0;
 // 조건식 : 값을 비교해서 결과를 참/거짓인지 판명해주는 연산자
 // 증감연산자 : ex. i++

 */

public class ForExample : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            Debug.Log($"안녕하세요.{i:4d}");
        }
        
        // 1부터 1,000까지 짝수만 더한 값을 구해보세요.

        int sum = 0;
        
        for (int i = 0; i <= 1000; i = i + 2)
        {
            sum += i;
        }
        
        Debug.Log(sum);
        
        // 2. 구구단 2단부터 9단까지 5단을 제외하고 중첩 반복문을 이용해서 출력해보세요.
        // 점프문 : 반복문 내부에서 흐름을 끊고 코드 실행 위치를 원하는 곳으로 이동(점프)해준다.
        // - break : 현재 실행중인 반복문이나 분기문의 실행을 중단할 때 사용. (종료/탈출 이라고 부르기도 함)
        // - countinue : 반복문 코드블록 내에서 현재 실행중인 코드 라인의 아래 코드들의 실행을 건너 뛸 때 사용

        for (int i = 2; i <= 9; i++)
        {
            if (i == 5)
            {
                continue;
            }
            
            for (int j = 1; j <= 9; j++)
            {
                
                Debug.Log($"{i} * {j} = {i * j}");
            }
        }
        
        // while 반복문
        
        
        
        Debug.Log("안녕하세요");
        
    }

}
