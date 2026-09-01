using UnityEngine;

public class SwitchCaseExample : MonoBehaviour
{
    void Start()
    {
        // 분기문
        // 내가 원하는 값과 같냐 다르냐에 따라서 실행할 코드 분기를 나눌 수 있다.
        
        /*
         switch (검사할 변수)
         {
            case 값1:
            {
                실행할 코드
                break;
            }
            
            case 값2:
            {
                실행할 코드
                break;
            }
            
            default:        // (옵션, 아무런 case에 해당하지 않는다면..)
            {
                실행할 코드
                break;
            }
         
         }
         
         */
        
        // [분기문(switch-case) 이용]
        
        // 1번 문제
        // 변수 today를 선언하고.. 요일에 따라
        // "오늘은 무슨 요일입니다!"를 출력해주는 코드를 작성하세요.

        
        
        string today = "Thursday";

        switch (today)
        {
            case "Monday":
            {
                Debug.Log("오늘은 월요일입니다!");
                break;
            }
            case "Tuesday":
            {
                Debug.Log("오늘은 화요일입니다!");
                break;
            }
            case "Wednesday":
            {
                Debug.Log("오늘은 수요일입니다!");
                break;
            }
            case "Thursday":
            {
                Debug.Log("오늘은 목요일입니다!");
                break;
            }
            case "Friday":
            {
                Debug.Log("오늘은 금요일입니다!");
                break;
            }
            case "Saturday":
            {
                Debug.Log("오늘은 토요일입니다!");
                break;
            }
            case "Sunday":
            {
                Debug.Log("오늘은 일요일입니다!");
                break;
            }
        }



        // 2번 문제
        // 변수 score을 선언하고.. 점수(0~100)에 따라
        // 100 : A+, 90~99 : A, 80~89 : B, 70~79 : C, 60~69 : D, 60미만 : F
        // 를 출력해주는 코드를 작성하세요.

        int score = 98;

        switch (score / 10)
        {
            case 10:
            {
                Debug.Log("A+");
                break;
            }
            case 9:
            {
                Debug.Log("A");
                break;
            }
            case 8:
            {
                Debug.Log("B");
                break;
            }
            
        }

    }
    
}
