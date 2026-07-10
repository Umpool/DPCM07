using UnityEngine;
using TMPro; // 텍스트 매시 프로 사용을 위해 추가

public class TopStatusBarController : MonoBehaviour
{
    [Header("[상단바 텍스트 구성요소]")]
    public TextMeshProUGUI goldText;      // 보유 골드 표시창
    public TextMeshProUGUI characterText; // 현재 출전 중인 메인 캐릭터 이름 표시창

    // [기획 완벽 연동]: 마을 화면이 탁 켜지는 순간, 상단바에 실제 유저의 최신 데이터를 연동해 줍니다.
    private void OnEnable()
    {
        Debug.Log("[System] 마을 전용 12시 고정 상단바가 정상 작동을 시작합니다.");
        UpdateStatusUI();
    }

    public void UpdateStatusUI()
    {
        // 1. 임시로 유저의 보유 재화를 텍스트창에 반영해 봅니다.
        if (goldText != null) goldText.text = "GOLD: 5,000";

        // 2. [30년 차 콜라보 공법]: 우리가 이전에 열심히 설계하고 선택창에서 들고 온 
        // 진짜 메인 캐릭터 ID 정보를 파티 매니저 주머니에서 쏙 꺼내와 상단바에 선명하게 박아줍니다!
        if (PartyManager.Instance != null && PartyManager.Instance.currentPartyList.Count > 0)
        {
            string currentMainHero = PartyManager.Instance.currentPartyList[0];

            if (characterText != null)
            {
                characterText.text = $"출전 영웅: ID {currentMainHero}";
            }
        }
        else
        {
            if (characterText != null) characterText.text = "출전 영웅: 없음";
        }
    }
}