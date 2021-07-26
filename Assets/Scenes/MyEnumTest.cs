using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEnumTest : MonoBehaviour
{
    [EnumName("测试枚举类型")]
    public MyEnum myEnum;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(myEnum.ToString());
        Debug.Log((int)myEnum);
    }

}
