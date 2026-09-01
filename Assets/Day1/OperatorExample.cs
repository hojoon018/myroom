using UnityEngine;

public class OperatorExample : MonoBehaviour
{
    void Start()
    {
        // 주어진 식을 계산에서 결과를 얻어내는 과정을 "연산"
        // 연산을 수행하는 기호를 "연산자"
        
        // 수학 연산자 (산술 연산자)
        // + - * / %
        int number1 = 200;
        int number2 = 300;
        int result = number1 + number2;
        
        Debug.Log(result);
        Debug.Log(200 + 300); // 리터럴, 임시값, R-Value
        
        // 대입 연산자 : 값을 대입(할당)하는 연산자 -> '='
        string myName = "박보검";
        
        // 증강 연산자 : 값이 1씩 증가 or 감소할 때 사용하는 연산자
        int age1 = 27;
        age1++;  // 후위 증강 연산자 -> age = age + 1;

        int age2 = 27;
        ++age2; // 전위 증강 연산자


        int age3 = 27;
        age3 += 3;  // age = age + 3;
        
        // 비교 연산자 : 양쪽 값을 비교하여 결과를 true / false 인지 판명해주는 연산자
        // ==, !=, >, >=, <, <=
        int student1Age = 20;
        int student2Age = 27;
        
        Debug.Log(student1Age == student2Age);
        Debug.Log(student1Age != student2Age);
        Debug.Log(student1Age > student2Age);
        Debug.Log(student1Age >= student2Age);
        Debug.Log(student1Age <= student2Age);
        
        // 논리 연산자 = 비교 연산자가 동시에 두 개 이상 사용할 때 쓰는 연산자
        // 비교 연산의 결과는 : true/false
        
        // (AND)   : &&  // 두 조건이 모두 참일때만 참
        // (OR)    : ||  // 두 조건 중 하나마 참이어도 참
        // (NOT)   : !   // 참은 거짓으로, 거짓은 참으로
        
        // 삼항 연산자 : 조건식을 사용해서 조건식이 참/거짓이냐에 따라 다른 값을 대입
        int health = 100;
        int damage = 50;
        
        // 변수 = 조건식 ? 참이면 대입할 값 : 거짓이면 대입할 값

        bool isDie = health - damage > 0 ? false : true;
        Debug.Log(isDie);


    }


}


