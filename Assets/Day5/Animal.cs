using UnityEngine;

public abstract class Animal    // 추상 클래스 : new 연산자를 이용해서 객체를 못 만듦..
{
    // protected : 외부에서는 접근 불가능하지만, 내부 혹은
    //              자식 클래스에서는 접근 가능
    protected string _name = string.Empty;
    protected float _height = 0f;
    protected double _weight = 0f;
    
    // virtual : 하위 클래스에서 재정의 가능함을 알려주는 키워드
    public abstract void Introduce(); // 추상 메서드 -> 구현이 없는 메소드이므로 하위 클래스에 무조건 구현을 해야하는 클래스

    public void Run()
    {
        Debug.Log($"[{_name} 뛰는 중!] 속도: {100 - (_height / _weight)}");
    }
}
