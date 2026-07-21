using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStorageManager : MonoBehaviour
{
    [Header("독립 파티창 슬롯 UI (0번이 맨 왼쪽 리더)")]
    [SerializeField] private List<Image> uniquePartySlots = new List<Image>();
    [SerializeField] private Sprite storageEmptySlotSprite;

    [Header("독립 격자 배치 UI (Grid Layout Group 부모)")]
    [SerializeField] private Transform storageGridParentGroup;
    [SerializeField] private GameObject storageCharacterSlotPrefab; // 만능 붕어빵 틀 프리팹

    [Header("독립 시너지 및 하단 정보 UI")]
    [SerializeField] private TextMeshProUGUI storageSynergyText;
    [SerializeField] private TextMeshProUGUI storageCountText;
    [SerializeField] private int storageMaxCount = 10;

    [Header("💡 [기획 원안 추가] 격자 및 파티 레이아웃 설정")]
    [Tooltip("가로로 배치할 최대 칸 수 (기획서 기준: 5)")]
    private int gridColumnCount = 5;

    [Header("Character Prefab Setup")]
    public GameObject[] characterPrefabs;

    private List<string> storageTempPartyList = new List<string>();

    private void OnEnable()
    {
        // 1. 싱글톤 보유 가방(UserCharacterInventory)으로부터 내가 가진 전체 캐릭터 ID를 내 창고 주머니로 완벽하게 이식합니다.
        if (UserCharacterInventory.Instance != null && UserCharacterInventory.Instance.ownedCharacterIDs != null)
        {
            storageOwnedCharacterIDs.Clear();
            foreach (string id in UserCharacterInventory.Instance.ownedCharacterIDs)
            {
                storageOwnedCharacterIDs.Add(id);
            }
        }

        // 2. 외부 파티 주머니(PartyManager)에 저장된 진짜 리더 캐릭터 ID 데이터를 내 임시 주머니로 복사합니다.
        if (PartyManager.Instance != null && PartyManager.Instance.currentPartyList != null)
        {
            storageTempPartyList.Clear();
            foreach (string id in PartyManager.Instance.currentPartyList)
            {
                storageTempPartyList.Add(id);
            }
        }

        // 3. 데이터 동기화가 완전히 끝났으므로 화면 UI들을 정석대로 다시 그립니다.
        RefreshUniqueStorageUI();
        RefreshUniquePartyUI();
    }


    /// <summary>
    /// [최종 혁신 엔진] 프로젝트 '캐릭터' 폴더에 들어있는 진짜 실물 데이터 시트(.asset)를 직접 찾아와 프리팹에 통째로 수동 도킹시킵니다!
    /// </summary>
    public void RefreshUniqueStorageUI()
    {
        // 1. 기존 창고 격자판 자식 오브젝트(오래된 카드들) 청소
        foreach (Transform child in storageGridParentGroup)
        {
            Destroy(child.gameObject);
        }

        // 2. 선택창에서 유저가 콕 집어 고른 나만의 고유 캐릭터 ID를 영리하게 꺼내옵니다.
        string currentMainHeroID = PlayerPrefs.GetString("SelectedCharacterID", "01");

        // 💡 [기획 원안 연동] 유저가 보유한 고유 캐릭터 리스트를 생성합니다.
        List<string> testOwnedIDs = new List<string> { currentMainHeroID };

        // 3. [우측 하단] 캐릭터 현재 보유수 / 최대보유수 텍스트 업데이트 (예: 1 / 10)
        if (storageCountText != null)
        {
            storageCountText.text = testOwnedIDs.Count.ToString() + " / " + storageMaxCount.ToString();
        }

        // 4. 프로젝트 '캐릭터' 폴더 안의 모든 기획서 데이터를 싹 긁어옵니다.
        CharacterData[] allDataSheets = Resources.LoadAll<CharacterData>("캐릭터");
        if (allDataSheets == null || allDataSheets.Length == 0)
        {
            allDataSheets = Resources.FindObjectsOfTypeAll<CharacterData>();
        }

        // 5. 내 가방을 뒤져서 파일 이름에 현재 유저가 고른 ID가 들어있는 진짜 문서를 찾아 격자판에 생성합니다!
        foreach (string charID in testOwnedIDs)
        {
            CharacterData originalData = null;
            foreach (var sheet in allDataSheets)
            {
                if (sheet != null && sheet.name.Contains(charID))
                {
                    originalData = sheet;
                    break;
                }
            }

            // 진짜 데이터 문서를 찾았다면 만능 '슬롯' 프리팹을 격자판 자식으로 소환합니다.
            if (originalData != null && storageCharacterSlotPrefab != null)
            {
                GameObject newCard = Instantiate(storageCharacterSlotPrefab, storageGridParentGroup);
                
                // 생성된 슬롯에 데이터를 심어줍니다.
                CharacterComponent cardComp = newCard.GetComponent<CharacterComponent>();
                if (cardComp != null)
                {
                    cardComp.SetCharacterData(originalData);
                }

                // 슬롯 겉면 이미지와 기획서에 명시된 고유 색상을 입혀줍니다.
                Image cardImage = newCard.GetComponent<Image>();
                if (cardImage != null && originalData.characterSprite != null)
                {
                    cardImage.sprite = originalData.characterSprite;
                    cardImage.color = new Color(originalData.characterColor.r, originalData.characterColor.g, originalData.characterColor.b, 1f);
                }
            }
        }
    }




    private void OnClickUniqueStorageCharacter(string characterID)
    {
        if (storageTempPartyList.Contains(characterID))
        {
            storageTempPartyList.Remove(characterID);
            Debug.Log($"[Storage] 파티에서 메인 캐릭터 제외 완료: {characterID}");
        }
        else
        {
            if (storageTempPartyList.Count >= uniquePartySlots.Count)
            {
                Debug.LogWarning("[Storage] 파티원이 이미 가득 찼습니다!");
                return;
            }

            storageTempPartyList.Add(characterID);
            Debug.Log($"[Storage] 파티 맨 왼쪽 리더 자리 자동 영입 성공: {characterID}");
        }

        RefreshUniquePartyUI();
    }

    public void RefreshUniquePartyUI()
    {
        // 1. 상단 슬롯 기화 및 기존 연출 잔여물 청소
        for (int i = 0; i < uniquePartySlots.Count; i++)
        {
            if (uniquePartySlots[i] != null)
            {
                uniquePartySlots[i].sprite = storageEmptySlotSprite;
                foreach (Transform child in uniquePartySlots[i].transform) 
                { 
                    Destroy(child.gameObject); 
                }
            }
        }

        // 2. 만능 '슬롯' 프리팹을 상단 칸에 소환하고, 하단에 생성된 진짜 데이터와 동기화합니다.
        CharacterComponent[] spawnedCards = storageGridParentGroup.GetComponentsInChildren<CharacterComponent>();
        
        for (int i = 0; i < storageTempPartyList.Count; i++)
        {
            if (i >= uniquePartySlots.Count) break;
            
            string partyCharID = storageTempPartyList[i];
            
            foreach (CharacterComponent card in spawnedCards)
            {
                if (card != null && card.myData != null && card.myData.characterID == partyCharID)
                {
                    if (uniquePartySlots[i] != null && storageCharacterSlotPrefab != null)
                    {
                        // 새 변수를 열어 부모 슬롯 자식 위치에 만능 프리팹을 생성합니다.
                        GameObject newSlotObj = Instantiate(storageCharacterSlotPrefab, uniquePartySlots[i].transform);
                        newSlotObj.transform.localPosition = Vector3.zero;
                        newSlotObj.transform.localScale = Vector3.one;

                        // 생성된 프리팹의 이미지 컴포넌트를 가져와 하단 원본 데이터의 그림과 색상을 똑같이 대입합니다.
                        Image slotImage = newSlotObj.GetComponent<Image>();
                        if (slotImage != null && card.myData.characterSprite != null)
                        {
                            slotImage.sprite = card.myData.characterSprite;
                            
                            // 💡 유저님이 지정하신 캐릭터의 고유 색상(빨강, 파랑 등)을 불투명하게 입혀줍니다.
                            slotImage.color = new Color(card.myData.characterColor.r, card.myData.characterColor.g, card.myData.characterColor.b, 1f);
                        }
                    }
                    break;
                }
            }
        }
        UpdateUniqueSynergyText();
    }



    private void UpdateUniqueSynergyText()
    {
        if (storageSynergyText == null) return;

        if (storageTempPartyList.Count == 0)
        {
            storageSynergyText.text = "현재 배치된 아군이 없습니다.\n하단 보관함에서 내 메인 캐릭터를\n터치해 파티를 구성해 보세요.";
            return;
        }

        string resultText = "";
        string leaderID = storageTempPartyList.Count > 0 ? storageTempPartyList[0] : ""; resultText += "<color=yellow>[👑 현재 배치된 메인 리더 효과]</color>\n";

        if (leaderID == "01" || leaderID == "001") resultText += "- [촌장의 위엄] 맨 왼쪽 리더의 격려로 아군 전체 공격력 +10%\n";
        else if (leaderID == "02" || leaderID == "002") resultText += "- [하마의 뚝심] 맨 왼쪽 리더의 존재감으로 아군 전체 체력 +15%\n";
        else if (leaderID == "03" || leaderID == "003") resultText += "- [악어의 투지] 맨 왼쪽 리더의 포효로 치명타 확률 +12%\n";
        else if (leaderID == "04" || leaderID == "004") resultText += "- [너구리의 지혜] 맨 왼쪽 리더의 영리함으로 코인 골드 획득량 +20%\n";
        else resultText += $"- 일반 리더 보너스: 아군 전체 이동 속도 +5%\n";

        storageSynergyText.text = resultText;
    }

    public void OnClickUniquePartySlot(int slotIndex)
    {
        if (slotIndex < storageTempPartyList.Count)
        {
            string removedCharID = storageTempPartyList[slotIndex];
            storageTempPartyList.RemoveAt(slotIndex);
            Debug.Log($"[Storage] 상단 파티장 터치 -> 파티에서 제외 완료: {removedCharID}");
            RefreshUniquePartyUI();
        }
    }

    public void OnClickReturnToTown()
    {
        Debug.Log("[Storage] 변경 사항 세이브 및 마을 복귀 가동");

        if (PartyManager.Instance != null)
        {
            PartyManager.Instance.currentPartyList.Clear();
            foreach (string charID in storageTempPartyList)
            {
                PartyManager.Instance.currentPartyList.Add(charID);
            }

            string combinedParty = string.Join(",", PartyManager.Instance.currentPartyList);
            PlayerPrefs.SetString("SelectedCharacterID", storageTempPartyList.Count > 0 ? storageTempPartyList[0] : "");
            PlayerPrefs.SetString("CurrentPartyListString", combinedParty);
            PlayerPrefs.Save();
        }

        PlayerPrefs.SetInt("IsReturningFromStorage", 1);
        PlayerPrefs.Save();

        CancelInvoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene("게임초반에서마을까지");
    }
}
