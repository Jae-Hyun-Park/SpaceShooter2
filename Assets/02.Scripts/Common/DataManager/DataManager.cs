using System.Collections;
using System.Collections.Generic;
// 파일 입출력 관련 namespace
using System.IO;
// 바이너리 파일 포맷 namespace
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using DataInfo;

public class DataManager : MonoBehaviour {

    // 파일이 저장될 물리적인 경로 및 파일명을 저장할 변수
    private string dataPath;

    // 파일 저장 경로와 파일명 지정 및 초기화
    public void Initialized()
    {
        dataPath = Application.persistentDataPath + "/gameData.dat";
    }

    // 데이터 저장 및 파일 생성
    public void Save(GameData gameData)
    {
        // 바이너리 파일 포맷을 위한 BinaryFormatter 생성 
        BinaryFormatter bf = new BinaryFormatter();
        // 데이터 저장을 위한 파일 생성
        FileStream file = File.Create(dataPath);

        // 파일에 저장할 클래스에 데이터 할당
        GameData data = new GameData();
        data.killCount      = gameData.killCount;
        data.speed          = gameData.speed;
        data.damage         = gameData.damage;
        data.equipedItem    = gameData.equipedItem;

        bf.Serialize(file, data);
        file.Close();
    }

    public GameData Load()
    {
        GameData data;
        if (File.Exists(dataPath))
        {
            // 파일이 존재하는경우 데이터를 불러옴
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            // GameData 클래스에 파일로부터 읽은 데이터를 받아옴
            data = (GameData)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            // 파일이 없는 경우 새로 생성해서 넘겨줌
            data = new GameData();
        }
        return data;
    }
}
