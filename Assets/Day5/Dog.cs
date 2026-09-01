using UnityEngine;

// 상속이란 : 부모(상위/기반/슈퍼) 클래스의 속성과 기능을 자식(하위/파생/서브) 클래스에게 물려받아 그대로 사용하거나 확장/변형 하는 것
// - 장점 :
// - 코드 재사용 (이미 만들어진 부모 클래스의 속성과 기능을 공유하므로 중복 최소화)
// - 확장 (자식 클래스만의 고유한 속성이나 기능 추가 가능)
// - 재정의 : 부모에게 물려 받은 메서드를 상황에 맞게 재정의(오버라이딩) 하여 사용 가능

public class Dog : Animal
{
    
    public Dog(string name, float height, float weight)
    {
        _name = name;
        _height = height;
        _weight = weight;
    }

    // override 쓰지 않으면 재정의가 아니라 메서드 쉐도잉(하이딩)하고 있는 것이다.
    // 재정의 문법은 override를 써야한다.
    // 재정의를 하면 가리키는 타입과 상관없이 실제 타입의 메서드를 호출한다.
    
    public override void Introduce()
    {
        Debug.Log($"멍멍~ 내 이름은 {_name}이다 멍~");
    }
    
}
