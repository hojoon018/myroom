using UnityEngine;

public class ForEachExample : MonoBehaviour
{
    private void Start()
    {
        // 문자열 : 문자를 순서대로 나'열'      (문자 배열)
        string myName = "황금독수리온세상을놀라게하다";
        Debug.Log(myName);
        Debug.Log(myName[0]);                   //황
        Debug.Log(myName[1]);                   //금
        Debug.Log(myName[myName.Length - 1]);   //다

        for (int i = 0; i < myName.Length; i++)
        {
            Debug.Log(myName[i]);
        }
        
        // foreach : 배열이나 컬렉션의 요소를 처음부터 끝까지 순서대로 간편하게 접근할 때 사용하는 반복문
        foreach (char c in myName)
        {
            Debug.Log(c);
        }
        
    }
    
}
