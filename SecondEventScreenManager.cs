using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SecondEventScreenManager : MonoBehaviour
{
    [Header("[UI 컴포넌트 및 버튼 연결]")]
    public TextMeshProUGUI storyText;   // 대사 출력 텍스트창
    public Button nextButton;           // 다음 대사 버튼

    [Header("[최종 마을 화면 패널]")]
    public GameObject villagePanel;     // 최종 이동할 마을 오브젝트

    [Header("[타이핑 연출 설정]")]
    public float typingSpeed = 0.05f;   // 글자 찍히는 속도

    private int currentStep = 0;        // 현재 시나리오 단계
    private bool isTyping = false;      // 현재 글자가 찍히는 중인가 체크
    private string fullText = "";       // 원본 전체 대사 임시 저장고
    private Coroutine colorCoroutine;   // 서서히 색을 물들여줄 타이머 주머니
    
    // [기획 핵심 변수]: 현재 글자가 지정된 색으로 물든 상태인지 체크하는 스위치
    private bool isColorFaded = false;  

    private void OnEnable()
    {
        currentStep = 0; // 화면이 켜지자마자 0단계 대사부터 가동합니다.
        isColorFaded = false;
        
        if (nextButton != null) nextButton.onClick.AddListener(OnClickNextButton);
        ExecuteCurrentStep();
    }

    private void OnDisable()
    {
        if (nextButton != null) nextButton.onClick.RemoveListener(OnClickNextButton);
    }

    public void OnClickNextButton()
    {
        // [규칙 1]: 글자가 타닥타닥 찍히는 중이면 문장을 한 번에 다 보여주고 멈춥니다 (스킵 기능)
        if (isTyping)
        {
            StopAllCoroutines();
            storyText.text = fullText;
            isTyping = false;
            return;
        }

        // [규칙 2]: 타이핑이 끝났는데 아직 색이 안 물들었다면? -> 스르륵 색을 바꿉니다!
        if (!isColorFaded)
        {
            StartFadeColorEffect();
            return;
        }

        // [규칙 3]: 색 물들기까지 완전히 끝난 상태에서 누르면 다음 대사 단계로 이동합니다.
        currentStep++;
        isColorFaded = false; // 새로운 단계를 위해 색상 스위치 꺼주기
        ExecuteCurrentStep();
    }
    // ... 파트 2로 바로 이어집니다.
    // ... 파트 1 코드 하단에 바로 이어서 붙여넣으세요
    private void ExecuteCurrentStep()
    {
        if (storyText != null) storyText.color = Color.white; // 텍스트 컬러 초기화
        switch (currentStep)
        {
            case 0: fullText = "네번째 텍스트입니다."; break;
            case 1: fullText = "다섯번째 텍스트입니다."; break;
            case 2: fullText = "여섯번째 텍스트입니다."; break;
            default:
                // 최종 시나리오 종료 후 자가 초기화 및 화면 전환
                currentStep = 0; 
                if (storyText != null) storyText.text = ""; 
                if (villagePanel != null) villagePanel.SetActive(true);
                this.gameObject.SetActive(false); 
                return;
        }
        StartTypingEffect(fullText);
    }

    private void StartFadeColorEffect()
    {
        Color targetColor = Color.white;
        switch (currentStep)
        {
            case 0: targetColor = Color.blue; break;
            case 1: targetColor = new Color(0.5f, 0f, 0.5f); break;
            case 2: targetColor = Color.red; break;
        }
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(FadeTextColorRoutine(targetColor, 0.8f));
    }

    private System.Collections.IEnumerator FadeTextColorRoutine(Color endColor, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = storyText.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            storyText.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null;
        }
        storyText.color = endColor;
        isColorFaded = true; // 색상 변경 완료 표시
    }

    private void StartTypingEffect(string targetText)
    {
        StopAllCoroutines();
        fullText = targetText;
        StartCoroutine(TypeTextRoutine());
    }

    private System.Collections.IEnumerator TypeTextRoutine()
    {
        isTyping = true;
        storyText.text = "";
        foreach (char letter in fullText)
        {
            storyText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
}
