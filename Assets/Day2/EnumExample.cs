using UnityEngine;

// Enum : 열거형
// 열거 : 순서대로 나열하다라는 뜻으로
// 관련 있는 상수들을 한 줄로 쭉~ 나열해서 묶어놓는(그룹화한) 형태를 열거형

// - 요일(월, 화, 수, 목, 금, 토, 일)
// - 계절(봄, 여름, 가을, 겨울)


// 요일 열거형 선언
// -> 요일을 기억하기 편하게 상수화/그룹화해서 선언해둔다.
// 내부적으로 정수 취급한다.
enum DayofWeek
{
    Monday,         //0
    Tuesday,        //1
    Wednesday,      //2
    Thursday,       //3
    Friday,         //4
    Saturday,       //5
    Sunday          //6
}

// 계절을 상수화/그룹화
enum Season
{
    Spring = 7,         //7
    Summer,             //8
    Fall = 6,           
    Winter              //
}

enum KeyboardCode
{
    W,
    A,
    S,
    D,
    Alpha0
}

public class EnumExample : MonoBehaviour
{
    void Start()
    {
        DayofWeek today = DayofWeek.Thursday;
        today = DayofWeek.Friday;

        if (today == DayofWeek.Thursday)
        {
            Debug.Log("오늘은 목요일입니다.");
        }

        Debug.Log(today);
        
        Debug.Log((int)today);
        
        Season todaySeason = Season.Summer;
        
        if (todaySeason == Season.Summer)
        {
            Debug.Log("여름입니다.");
        }
        
        KeyboardCode currentInputKeyboardCode = KeyboardCode.A;
        if (currentInputKeyboardCode == KeyboardCode.A)
        {
            Debug.Log("왼쪽으로 이동 중...");
        }

    }
    
}
