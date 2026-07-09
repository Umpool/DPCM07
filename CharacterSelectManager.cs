using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    public TextMeshProUGUI nameText;       // 캐릭터 이름 텍스트창
    public TextMeshProUGUI infoText;       // 캐릭터 정보 통합 텍스트창
    public Button confirmButton;           // 출발하기 버튼

    [Header("시각적 파티창 및 프리팹 조립 칸")]
    public Transform partyPanel;           // 💡 실시간으로 카드가 생성될 파티창 (빈 오브젝트 방)
    public GameObject cardPrefab;          // 💡 파티창에 띄워줄 캐릭터 카드 프리팹

    [Header("화면 전환용 오브젝트")]
    public GameObject secondEventPanel;

    private CharacterData selectedCharacter = null;
    private GameObject lastHiddenButton = null; // 💡 직전에 숨겨진 버튼 오브젝트를 기억하는 상자
}
    // [Part 2: 실시간 파티 생성 및 버튼 숨김/복원 연출 기능]
    public void OnSelectCharacter(CharacterData data, GameObject clickedButton)
    {
        if (data == null || clickedButton == null) return;

        // 1. [기존 버튼 복원 및 파티창 청소] 직전에 선택된 영웅이 있다면 복구합니다.
        if (lastHiddenButton != null)
        {
            lastHiddenButton.SetActive(true); // 숨겨졌던 이전 버튼 다시 보이게 하기
        }
        
        // 파티창 방 내부를 깨끗하게 비웁니다 (기존에 생성된 카드 삭제)
        foreach (Transform child in partyPanel)
        {
            Destroy(child.gameObject);
        }

        // 2. [새 영웅 선택 처리]
        selectedCharacter = data;
        lastHiddenButton = clickedButton;
        clickedButton.SetActive(false); // 지금 누른 캐릭터 버튼은 화면에서 즉시 숨김!

        // 3. [파티창에 새 카드 소환] 프리팹을 복사해서 파티창 방의 자식으로 쏙 집어넣습니다.
        if (cardPrefab != null && partyPanel != null)
        {
            GameObject newCard = Instantiate(cardPrefab, partyPanel);
            
            // 소환된 카드가 "내 데이터는 이것이다"라고 인지하도록 데이터를 심어줍니다.
            CharacterComponent cardComp = newCard.GetComponent<CharacterComponent>();
            if (cardComp != null)
            {
                cardComp.myData = selectedCharacter;
            }
        }

        // 4. 큰 정보창 텍스트 및 출발하기 자물쇠 해제
        if (nameText != null) nameText.text = selectedCharacter.characterName;
        if (infoText != null)
        {
            infoText.text = $"종족: {selectedCharacter.characterRace}\n\n" +
                            $"[전투 능력치] HP: {selectedCharacter.hp} / ATK: {selectedCharacter.attackPower} / DEF: {selectedCharacter.defense}\n\n" +
                            $"[고유 능력: {selectedCharacter.uniqueSkillName}]\n{selectedCharacter.uniqueSkillDescription}\n\n" +
                            $"소속 시너지: {selectedCharacter.synergySystem}\n\n" +
                            $"[캐릭터 스토리]\n{selectedCharacter.characterInfo}";
        }

        // 기존 장부 연동 및 버튼 활성화
        if (PartyManager.Instance != null)
        {
            PartyManager.Instance.currentPartyList.Clear();
            PartyManager.Instance.AddToParty(selectedCharacter.characterID);
        }
        if (confirmButton != null) confirmButton.interactable = true;
    }

    // 5. [출발하기 완료 시] 유저의 최종 보유 캐릭터로 판정하고 평생 저장!
    public void OnClickConfirmSelection()
    {
        if (selectedCharacter == null) return;
        
        // 기기에 ID를 저장하여 이 캐릭터를 보유중인 것으로 확정합니다.
        PlayerPrefs.SetString("UserOwnedCharacterID", selectedCharacter.characterID);
        PlayerPrefs.Save();

        // 다음 이벤트 화면으로 전환
        if (secondEventPanel != null) secondEventPanel.SetActive(true);
        gameObject.SetActive(false); 
    }
}
