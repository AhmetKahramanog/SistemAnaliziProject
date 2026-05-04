using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SqlClient;
using TMPro;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviour
{
    string connectionString = @"Server=localhost\SQLEXPRESS;Database=GameTest;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

    public static int activePlayerID = -1;

    [SerializeField] private TMP_InputField usernameBox;
    [SerializeField] private TMP_InputField passwordBox;

    public int lastSaveGold;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ClickedButtonLogin()
    {
        Login(usernameBox.text, passwordBox.text);
    }

    public void ClickedButtonRegister()
    {
        Register(usernameBox.text, passwordBox.text);
    }

    public void Login(string username, string password)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT Oyuncu_ID FROM Oyuncular WHERE Kullanici_Adi = @username AND password = @password";
            using (SqlCommand command = new SqlCommand(query,connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    activePlayerID = System.Convert.ToInt32(result);
                    Debug.Log($"Login success : {activePlayerID}");
                    GetLastScoreFromDatabase();
                    SceneManager.LoadScene(1);
                }
                else
                {
                    Debug.Log("Error : Username or password is false");
                }
            }
        }
    }

    public void Register(string username, string password)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string checkQuery = "SELECT COUNT(*) FROM Oyuncular WHERE Kullanici_Adi = @username";
            using (SqlCommand checkCommand = new SqlCommand(checkQuery,connection))
            {
                checkCommand.Parameters.AddWithValue("@username", username);
                int userCount = (int)checkCommand.ExecuteScalar();

                if (userCount > 0)
                {
                    Debug.Log("Error : This username already taken");
                    return;
                }
                string insertQuery = "INSERT INTO Oyuncular (Kullanici_Adi,password) VALUES (@username,@password)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery,connection))
                {
                    insertCommand.Parameters.AddWithValue("@username", username);
                    insertCommand.Parameters.AddWithValue("@password", password);

                    insertCommand.ExecuteNonQuery();
                    Debug.Log("Register success");
                }
            }
        }
    }
    public void ScoreSave(int gold)
    {
        if (activePlayerID == -1)
        {
            Debug.Log("Error : No played are logged in, Score not save");
            return;
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Skorlar (Oyuncu_ID, Toplanan_Altin,Oynama_Tarihi) VALUES (@id,@altin,GETDATE())";
            using (SqlCommand command = new SqlCommand(query,connection))
            {
                command.Parameters.AddWithValue("@id", activePlayerID);
                command.Parameters.AddWithValue("@altin", gold);

                command.ExecuteNonQuery();
                Debug.Log($"Conglrations you have totaly {gold} gold saved in database");
            }
        }
    }
    private void GetLastScoreFromDatabase()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string scoreQuery = "SELECT MAX(Toplanan_Altin) FROM Skorlar WHERE Oyuncu_ID = @id";
            using (SqlCommand command = new SqlCommand(scoreQuery,connection))
            {
                command.Parameters.AddWithValue("@id", activePlayerID);
                object scoreResult = command.ExecuteScalar();
                if (scoreResult != System.DBNull.Value && scoreResult != null)
                {
                    lastSaveGold = System.Convert.ToInt32(scoreResult);
                }
                else
                {
                    lastSaveGold = 0;
                }
            }
        }
    }
}
