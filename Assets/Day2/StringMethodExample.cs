using UnityEngine;

public class StringMethodExample : MonoBehaviour
{
    // 문자열을 문자열만의 여러 가지 속성과 기능을 가지고 있다.
    // 서식, 길이, 탐색, 변형, 분할
    void Start()
    {
        // 길이 속성
        string name = " 황금독수리 새상을 놀라게 하다.  ";
        Debug.Log(name.Length);
        
        // 문자열은 불변식이므로 문자가 수정될 때마다 새로 메모리를 할당한다.
        // 그러므로 잘 해라
        
        // 탐색 기능 : 문자열 안에서 특정 문자열이 있는지 없는지, 있다면 어디인지 등등을 알 수가 있다.
        int index = name.IndexOf("놀");
        Debug.Log(index);
        bool isHwang = name.StartsWith("황");
        Debug.Log(isHwang);
        bool isContainEagle = name.Contains("독수리");
        Debug.Log(isContainEagle);
        
        // 변형 기능 : 대소문자 변환 혹은 추가, 대체, 삭제
        name = name.Trim();  //공백 제거
        Debug.Log(name);

        name = name.Insert(6, "온");  // 중간 삽입
        Debug.Log(name);

        name = name.Remove(name.IndexOf('.'));
        name = name.Replace(".", "");
        name = name.Replace("새", "세");
        Debug.Log(name);

        string name2 = "karina";
        name2 = name2.ToUpper();
        Debug.Log(name2);
        name2 = name2.ToLower();
        Debug.Log(name2);
    }

    
}


    // Int : 4바이트
    // Double : 8바이트
    // string : 2 * 문자열의 길이 바이트
    // string name = "김호준"
    // string otherName = "황금독수리온세상을놀라게하다."
