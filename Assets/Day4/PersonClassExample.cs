using UnityEngine;

public class PersonClassExample : MonoBehaviour
{
    private void Start()
    {
        // 자료형 변수명 = 값
        // 클래스 클래스명 = new 클래스
        
        Chair chair1 = new Chair();
        chair1.Color = "푸른색";


        Person person1 = new Person("김호준", 25, 172.4f, 77d, false);
        
        // Debug.Log(person1.Name);    // 현실에서는 불가능
        // person1.Name = "이호준";       // 현실에서는 불가능
        // person1.Age = 245;
        
        Debug.Log(person1.HasGlasses);
        
        person1.Introduce();
        person1.Run();
        person1.SeatDown(chair1);
        
        Person person2 =  new Person("김민재", 23, 178.4f, 79d, true);
        person2.Introduce();
        person2.Run();
        person2.SeatDown();
        
        person1.Punch(person2);


        Person person3 = null;  // 메모리 주소를 가리킬 변수만 만들었지 알맹이는 존재 하지 않음
        // null이란 존재하지 않는다는 뜻
        
        // 객체지향 프로그래밍에서 제임스 고슬링이 땅을 치고 후회하는 문법 중 하나
        // 가장 빈번하게 오류를 일으키는 문법
        
        person1.Punch(person3);

    }
    
}
