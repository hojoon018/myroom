using UnityEngine;

public class StringParseExample : MonoBehaviour
{
    void Start()
    {
        // 문자열을 int로 변환
        string stringAge = "38";
        int intage2 = int.Parse(stringAge);
        
        int intAge = 12;
        bool result1 = int.TryParse(stringAge, out intAge);         //입력 받을 때는 TryParse를 사용하여 시도할 수 있는지 확인
        if (result1 == true)
        {
            Debug.Log(intAge);
        }
        else
        {
            Debug.Log(intAge);
            Debug.Log("변환에 실패했습니다.");
        }

        // float -> 문자열
        float floatHeight = 174f;
        string stringHeight = floatHeight.ToString();
        
        // 문자열을 float로 변환
        string stringWeight = "70.2kg";
        float floatWeight = 0f;
        bool result2 = float.TryParse(stringWeight, out floatWeight);
        if (result2)
        {
            Debug.Log(floatWeight);
        }
        else
        {
            Debug.Log("변환에 실패했습니다.");
        }

    }
    
}
