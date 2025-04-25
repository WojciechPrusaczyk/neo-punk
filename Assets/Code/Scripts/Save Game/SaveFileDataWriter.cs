using UnityEngine;
using System;
using System.IO;
using System.Text;

public class SaveFileDataWriter
{
    public string saveDataDirectoryPath = "";
    public string saveFileName = "";

    private const string EncryptionMarker = "ENC::"; // nawet nie pytaj po co, po prostu

    private string GenerateKey() // to nie jest zbyt bezpiecznie, ale who cares
    {
        char[] keyParts = new char[12];
        keyParts[0] = (char)('S' + 2);
        keyParts[1] = (char)('e' - 1);
        keyParts[2] = (char)('c' * 1);
        keyParts[3] = (char)('r' + 5);
        keyParts[4] = (char)('e' + 0);
        keyParts[5] = (char)('t' - 3);
        keyParts[6] = (char)('K' + 1);
        keyParts[7] = (char)('e' + 4);
        keyParts[8] = (char)('y' - 2);
        keyParts[9] = (char)('!' + 1);
        keyParts[10] = (char)('9' - 1);
        keyParts[11] = (char)('$' + 0);
        return new string(keyParts);
    }

    // Metoda do szyfrowania i deszyfrowania danych
    private string EncryptDecrypt(string data)
    {
        string key = GenerateKey();
        StringBuilder output = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            output.Append((char)(data[i] ^ key[i % key.Length]));
        }
        return output.ToString();
    }

    // Metoda do sprawdzania, czy plik zapisu istnieje
    public bool CheckToSeeIfFileExists()
    {
        string filePath = Path.Combine(saveDataDirectoryPath, saveFileName);
        return File.Exists(filePath);
    }

    // Metoda do usuwania pliku zapisu
    public void DeleteSaveFile()
    {
        try
        {
            string filePath = Path.Combine(saveDataDirectoryPath, saveFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error deleting file {saveFileName}: {ex}");
        }
    }

    // Metoda do tworzenia nowego pliku zapisu
    public void CreateNewCharacterSaveFile(CharacterSaveData characterData, bool encrypt)
    {
        string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);
        string finalDataToWrite;

        try
        {
            // Stworzenie katalogu
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            string dataToStore = JsonUtility.ToJson(characterData, true);

            // Czy to trzeba wyjaœniaæ?
            if (encrypt)
            {
                string encryptedData = EncryptDecrypt(dataToStore);
                finalDataToWrite = EncryptionMarker + encryptedData;
            }
            else
            {
                finalDataToWrite = dataToStore;
            }

            // Zapisanie danych do pliku
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter fileWriter = new StreamWriter(stream))
                {
                    fileWriter.Write(finalDataToWrite);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR WHILST TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED " + savePath + "\n" + ex);
        }
    }

    // Metoda do ³adowania pliku zapisu
    public CharacterSaveData LoadSaveFile()
    {
        CharacterSaveData characterData = null;
        string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if (File.Exists(loadPath))
        {
            try
            {
                string rawData = "";
                using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        rawData = reader.ReadToEnd();
                    }
                }

                string dataToLoad;
                if (!string.IsNullOrEmpty(rawData) && rawData.StartsWith(EncryptionMarker))
                {
                    string encryptedData = rawData.Substring(EncryptionMarker.Length);
                    dataToLoad = EncryptDecrypt(encryptedData);
                }
                else
                {
                    dataToLoad = rawData;
                }

                // Jeœli dane nie s¹ puste, deserializujemy je
                if (!string.IsNullOrWhiteSpace(dataToLoad))
                {
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                else
                {
                    Debug.LogWarning($"Save file {loadPath} contained empty or invalid data after processing.");
                }

            }
            catch (Exception ex)
            {
                Debug.LogError("ERROR WHILST TRYING TO LOAD CHARACTER DATA (Could be decryption/deserialization failure), GAME NOT LOADED " + loadPath + "\n" + ex);
                characterData = null;
            }
        }
        return characterData;
    }
}
