using System.Collections;
using System.Collections.Generic;

namespace DataInfo
{
    [System.Serializable]
    public class GameData
    {
        public int killCount = 0;       //사망한 적 캐릭터의 수
        public float hp = 120.0f;       //주인공의 초기 생명력
        public float damage = 25.0f;    //총알의 기본 데미지
        public float speed = 6.0f;      //주인공의 이동속도
        public List<Item> equipedItems = new List<Item>();
    }

    [System.Serializable]
    public class Item
    {
        public enum ItemType { HP, SPEEDUP, GRENADE, DAMAGEUP }   // 아이템 종류
        public enum ItemCalc { INC_VALUE, PERCENT}              // 계산 방식 정의
        public ItemType itemType;                               // 아이템 종류 변수 선언
        public ItemCalc itemCalc;                               // 계산방식 종류 변수 선언
        public string name;                                     // 아이템 이름
        public string desc;                                     // 아이템 설명
        public float value;                                     // 계산 값
    }
}
