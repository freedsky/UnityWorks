using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ÍøÂçÏÂÔØ
/// </summary>

public class NetDown : MonoBehaviour
{
    public Material mat;
    private string localAddress;
    private string webAddress;

    void Start()
    {
        localAddress = "file://" + Application.dataPath + "/GameAssets/Textures/Environment/Textures/FoliageColor.png";
        webAddress = "https://image.baidu.com/search/detail?ct=503316480&z=undefined&tn=baiduimagedetail&ipn=d&word=%E5%9B%BE%E7%89%87&step_word=&lid=9820755664410934089&ie=utf-8&in=&cl=2&lm=-1&st=undefined&hd=undefined&latest=undefined&copyright=undefined&cs=2690606835,3390810145&os=2372291151,5334087&simid=55533261,690502700&pn=1&rn=1&di=7308398814245683201&ln=1581&fr=&fmq=1711693562331_R&fm=&ic=undefined&s=undefined&se=&sme=&tab=0&width=undefined&height=undefined&face=undefined&is=0,0&istype=0&ist=&jit=&bdtype=0&spn=0&pi=0&gsm=1e&objurl=https%3A%2F%2Fpic.rmb.bdstatic.com%2Fbjh%2F914b8c0f9814b14c5fedeec7ec6615df5813.jpeg&rpstart=0&rpnum=0&adpicid=0&nojc=undefined&dyTabStr=MCwzLDEsMiw0LDYsNSw4LDcsOQ%3D%3D";
    }

    private void OnGUI()
    {
        if (GUILayout.Button("NetLoad")) 
        {
            StartCoroutine(Load(webAddress));
        }
    }

    IEnumerator Load(string address) 
    {
        WWW www = new WWW(address);
        yield return www;
        mat.mainTexture = www.texture;
        www.Dispose();
    }

    void Update()
    {
        
    }
}
