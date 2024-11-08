using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using Unity.VisualScripting;

public class GameLogic : NetworkBehaviour
{
    public Button[] cellButtons;
    public TMP_Text statusText;
    public Button restartButton;

    private static NetworkString<_32> currentPlayer { get; set; } = "X";  
    [Networked] private NetworkString<_32> Button0 { get; set; }
    [Networked] private NetworkString<_32> Button1 { get; set; }
    [Networked] private NetworkString<_32> Button2 { get; set; }
    [Networked] private NetworkString<_32> Button3 { get; set; }
    [Networked] private NetworkString<_32> Button4 { get; set; }
    [Networked] private NetworkString<_32> Button5 { get; set; }
    [Networked] private NetworkString<_32> Button6 { get; set; }
    [Networked] private NetworkString<_32> Button7 { get; set; }
    [Networked] private NetworkString<_32> Button8 { get; set; }

    public override void Spawned()
    {
        base.Spawned();

        // Set up button listeners
        for (int i = 0; i < cellButtons.Length; i++)
        {
            int index = i;
            cellButtons[i].onClick.AddListener(() => OnClickCell(index));
        }

        UpdateStatusText();
    }
    
    private void OnClickCell(int index)
    {
        if (IsPlayerTurn() && IsCellEmpty(index))
        {
            Debug.Log("OnClickCell: Valid move");

            if (Runner.LocalPlayer.PlayerId == 1)
            {
                RPC_SetCellValue(index, "X");
            }
            else 
            {
                RPC_SetCellValue(index, "O");
            
            }


            RPC_UpdateButtonText(index, (string)currentPlayer);

            Debug.Log("Above check win condition ");
            if (CheckWinCondition())
            {
                RPC_UpdateStatusText($"Player {(string)currentPlayer} Wins!");
                RPCDisableAllButtons();
            }
            else if (IsBoardFull())
            {
                RPC_UpdateStatusText("It's a Draw!");
                restartButton.interactable = true;
                RPC_RestartButtonIntractTrue();

            }
            else
            {
                SwitchPlayerRPC();
                RPC_UpdateStatusText($"Player {(string)currentPlayer}'s Turn");
            }
        }
    }

    private bool IsPlayerTurn()
    {
        if (Runner.LocalPlayer.PlayerId == 1 && currentPlayer == "X")
        {
            return true;
        }
        if (Runner.LocalPlayer.PlayerId == 2 && currentPlayer == "O")
        {
            return true;
        }
        return false;
    }

    private bool IsCellEmpty(int index)
    {
        NetworkString<_32> cellValue = GetCellValue(index);
        return (string)cellValue == "";
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdateButtonText(int index, string playerSymbol)
    {
        TMP_Text buttonText = cellButtons[index].GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = playerSymbol;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdateStatusText(string message)
    {
        statusText.text = message;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_RestartButtonIntractTrue()
    {
        restartButton.interactable = true;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_RestartButtonIntractFalse()
    {
        restartButton.interactable =false;
    }
    private NetworkString<_32> GetCellValue(int index)
    {
        switch (index)
        {
            case 0: return Button0;
            case 1: return Button1;
            case 2: return Button2;
            case 3: return Button3;
            case 4: return Button4;
            case 5: return Button5;
            case 6: return Button6;
            case 7: return Button7;
            case 8: return Button8;
            default: return "";
        }
        //Debug.LogWarning($"Getting value for index {index}");
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetCellValue(int index, string value)
    {
        Debug.Log($"Setting value for index {index} to {value}");
        switch (index)
        {
            case 0: Button0 = value; break;
            case 1: Button1 = value; break;
            case 2: Button2 = value; break;
            case 3: Button3 = value; break;
            case 4: Button4 = value; break;
            case 5: Button5 = value; break;
            case 6: Button6 = value; break;
            case 7: Button7 = value; break;
            case 8: Button8 = value; break;
        }

        Debug.LogWarning($"SetCellValue: Index {index}, Value: {(string)value}");
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void SwitchPlayerRPC()
    {
        Debug.Log("SwitchPlayer: Changing turn");
        
        currentPlayer = (currentPlayer == "X") ? "O" : "X";
        Debug.Log($"Current player is now: {(string)currentPlayer}");
    }

    private void UpdateStatusText()
    {

        statusText.text = $"Player {(string)currentPlayer}'s Turn{Runner.LocalPlayer.PlayerId}";
    }




    private  NetworkBool CheckWinCondition()
    {

        Debug.Log($"Checking win condition for player: {(string)currentPlayer}");

    int[][] winningCombinations = new int[][]
    {
        new int[] {0, 1, 2},
        new int[] {3, 4, 5},
        new int[] {6, 7, 8},
        new int[] {0, 3, 6},
        new int[] {1, 4, 7},
        new int[] {2, 5, 8},
        new int[] {0, 4, 8},
        new int[] {2, 4, 6}
    };

    string[] board = {
        Button0.ToString(), Button1.ToString(), Button2.ToString(),
        Button3.ToString(), Button4.ToString(), Button5.ToString(),
        Button6.ToString(), Button7.ToString(), Button8.ToString()
    };

    string playerSymbol = (string)currentPlayer;
    Debug.Log($"Board State: {string.Join(", ", board)}");
    Debug.LogError($"Player Symbol: {playerSymbol}");

    /*foreach (var combination in winningCombinations)
    {
        Debug.Log($"Checking combination: {combination[0]}, {combination[1]}, {combination[2]}" + playerSymbol);
        if (board[combination[0]] == (string)playerSymbol &&
            board[combination[1]] == (string)playerSymbol &&
            board[combination[2]] == (string)playerSymbol)
        {
            Debug.Log($"Player {playerSymbol} wins!");

            return true;
        }   
    }*/
    foreach (var combination in winningCombinations)
    {
        Debug.Log($"Checking combination: {combination[0]}, {combination[1]}, {combination[2]}" + playerSymbol);
        if (board[combination[0]] =="X"  &&
            board[combination[1]] == "X" &&
            board[combination[2]] == "X")
        {
            Debug.Log($"Player X wins!");
            RPC_RestartButtonIntractTrue();

            return true;
        }   
    }
    foreach (var combination in winningCombinations)
    {
        Debug.Log($"Checking combination: {board[combination[0]]}, {board[combination[1]]}, {board[combination[2]]}" + playerSymbol);
        if (board[combination[0]] =="O"  &&
            board[combination[1]] == "O" &&
            board[combination[2]] == "O")
        {
            Debug.Log($"Player O wins!");
            RPC_RestartButtonIntractTrue();

            return true;
        }   
    }
    return false;
    }


    private bool IsBoardFull()
    {
        return (string)Button0 != "" && (string)Button1 != "" && (string)Button2 != "" &&
               (string)Button3 != "" && (string)Button4 != "" && (string)Button5 != "" &&
               (string)Button6 != "" && (string)Button7 != "" && (string)Button8 != "";
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPCDisableAllButtons()
    {
        foreach (var button in cellButtons)
        {
            button.interactable = false;
        }
    }
    public void OnRestartButtonClicked()
    {
        RPC_RestartGame();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_RestartGame()
    {
        // Reset board cells
        Button0 = Button1 = Button2 = Button3 = Button4 = Button5 = Button6 = Button7 = Button8 = "";

        // Reset UI elements
        foreach (var button in cellButtons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = "";
            button.interactable = true;
        }

        RPC_RestartButtonIntractFalse();

        // Hide the restart button
       // resta.gameObject.SetActive(false);

        // Reset game state
        currentPlayer = "X";
        RPC_UpdateStatusText("Player X's Turn");
    }

}




/*
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Logic : MonoBehaviour
{
    public Button[] buttons; 
    private string currentPlayer = "X"; 
    private string[] board = new string[9];
    public TextMeshProUGUI winnerText;
    public Button restartbtn;

    void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; 
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
            board[i] = "";
        }
        winnerText.text = "";
        restartbtn.onClick.AddListener(RestartGame); 
        restartbtn.gameObject.SetActive(false);
    }

    void OnButtonClick(int index)
    {
        
        if (board[index] == "")
        {
            board[index] = currentPlayer;
            TextMeshProUGUI buttonText = buttons[index].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = currentPlayer;

            if (CheckForWinner())
            {
                Debug.Log(currentPlayer + " wins!");
                winnerText.text = "   "+currentPlayer + "\n  wins!";
                EndGame();
                DisableButtons();
            }

            else if (IsBoardFull())
            {
                winnerText.text = "oops Draw!";
                EndGame();
            }

            else
            {   currentPlayer = (currentPlayer == "X") ? "O" : "X";
            }
        }
    }

    bool CheckForWinner()
    {
        if (board[0] == currentPlayer && board[1] == currentPlayer && board[2] == currentPlayer)
            return true;
        if (board[3] == currentPlayer && board[4] == currentPlayer && board[5] == currentPlayer)
            return true;
        if (board[6] == currentPlayer && board[7] == currentPlayer && board[8] == currentPlayer)
            return true;

        if (board[0] == currentPlayer && board[3] == currentPlayer && board[6] == currentPlayer)
            return true;
        if (board[1] == currentPlayer && board[4] == currentPlayer && board[7] == currentPlayer)
            return true;
        if (board[2] == currentPlayer && board[5] == currentPlayer && board[8] == currentPlayer)
            return true;

        if (board[0] == currentPlayer && board[4] == currentPlayer && board[8] == currentPlayer)
            return true;
        if (board[2] == currentPlayer && board[4] == currentPlayer && board[6] == currentPlayer)
            return true;

        return false; 
    }

    bool IsBoardFull()
    {
    
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i] == "") return false; 
        }
        return true; 
    }

    void EndGame()
    {
        DisableButtons();
        restartbtn.gameObject.SetActive(true); 
    }

    void DisableButtons()
    {
        foreach (Button btn in buttons)
        {
            btn.interactable = false; // Disable button clicks
        }
    }

    void RestartGame()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            board[i] = ""; 
            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = ""; 
            buttons[i].interactable = true; 
        }

        currentPlayer = "X"; 
        winnerText.text = ""; 
        restartbtn.gameObject.SetActive(false); 
    }
}
*/
