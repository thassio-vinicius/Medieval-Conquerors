using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class UIController : MonoBehaviour
{
    public Color naturalColor;
    public TMP_InputField input;
    public TMP_Text placeholderText;
    public Animator knight, king, samurai, paladin;
    public GameObject panel, charactersList;
    public GameObject inputErrorText;
    string playerSelected;
    string text = "Enter a nickname";

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayGame()
    {
        bool inputTextNull = input.text == "" || input.text == null;
        bool playerSelectedNull = playerSelected == "" || playerSelected == null;
        if (inputTextNull || playerSelectedNull)
        {
            inputErrorText.SetActive(inputTextNull);
        }
        else
        {
            

            GameManager.instance.characterSelected = playerSelected;
            GameManager.instance.nickname = input.text;

            SceneManager.LoadScene("Lobby");
        }
    }

    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    public void ClosePanel()
    {
        DeselectButtons(charactersList.GetComponentsInChildren<Button>().ToList(), charactersList.GetComponentInChildren<Button>());
        placeholderText.text = text;
        input.text = "";
        panel.SetActive(false);
        inputErrorText.SetActive(false);
        samurai.enabled = false;
        king.enabled = false;
        knight.enabled = false;
        paladin.enabled = false;
        playerSelected = "";

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SamuraiSelect()
    {
        samurai.enabled = true;
        king.enabled = false;
        knight.enabled = false;
        paladin.enabled = false;
        playerSelected = "Samurai";

        HighlightButton(samurai.GetComponentInParent<Button>());

        samurai.Play("Idle");
    }

    public void KingSelect()
    {
        samurai.enabled = false;
        king.enabled = true;
        knight.enabled = false;
        paladin.enabled = false;
        playerSelected = "King";

        HighlightButton(king.GetComponentInParent<Button>());


        king.Play("Idle");

    }

    public void KnightSelect()
    {
        samurai.enabled = false;
        king.enabled = false;
        knight.enabled = true;
        paladin.enabled = false;
        playerSelected = "Knight";

        HighlightButton(knight.GetComponentInParent<Button>());


        knight.Play("Idle");

    }

    public void PaladinSelect()
    {
        samurai.enabled = false;
        king.enabled = false;
        knight.enabled = false;
        paladin.enabled = true;
        playerSelected = "Paladin";

        HighlightButton(paladin.GetComponentInParent<Button>());


        paladin.Play("Idle");

    }

    public void OnInputFieldPressed()
    {
        placeholderText.text = "";
        inputErrorText.SetActive(false);
    }

    public void OnInputFieldDeselected()
    {
        if (panel.activeSelf) placeholderText.text = input.text == null || input.text == "" ? text : "";
    }

    void HighlightButton(Button button)
    {
        Button[] otherButtons = charactersList.GetComponentsInChildren<Button>();

        List<Button> list = otherButtons.ToList();

        list.Remove(button);

        DeselectButtons(list, button);

        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = Color.black;
        colors.highlightedColor = button.colors.highlightedColor;
        colors.pressedColor = button.colors.pressedColor;
        colors.selectedColor = button.colors.selectedColor;
        button.colors = colors;
    }

    void DeselectButtons(List<Button> list, Button button)
    {

        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = naturalColor;
        colors.highlightedColor = button.colors.highlightedColor;
        colors.pressedColor = button.colors.pressedColor;
        colors.selectedColor = button.colors.selectedColor;

        list.ForEach(b => b.colors = colors);

    }

}
