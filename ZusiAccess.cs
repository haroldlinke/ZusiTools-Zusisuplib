using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System;
using System.Reflection.Metadata;

namespace Zusisuplib
{
    public class Zusiaccess
    {
  
        static public void ZUSI_write_ext_menuval_to_Regkey(string keyVal, int EntryIdx = 0, string BezeichnerSprache = "Deutsch", string Bezeichnertext = "", string Vatermenu = "", int MenuIndex = 5, string Datei = "", string? Parameter = "")
        {
            RegistryKey? key;

            try
            {
                key = Registry.CurrentUser.OpenSubKey(keyVal, true);
                if (key != null)
                {
                    Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} found");
                }
                else
                    Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} NOT found");
                try
                {
                    key = Registry.CurrentUser.CreateSubKey(keyVal);
                    Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} created");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Error in create_ZUSI_menu_entry {e}");
                    return;
                }
            }
            catch
            {
                Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} NOT found");
                try
                {
                    key = Registry.CurrentUser.CreateSubKey(keyVal);
                    Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} created");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Error in create_ZUSI_menu_entry {e}");
                    return;
                }
            }

            try
            {
                key.SetValue("BezeichnerSprache" + EntryIdx.ToString(), BezeichnerSprache, RegistryValueKind.String);
                key.SetValue("BezeichnerText" + EntryIdx.ToString(), Bezeichnertext, RegistryValueKind.String);
                key.SetValue("Vatermenu", Vatermenu, RegistryValueKind.String);
                key.SetValue("MenuIndex", MenuIndex, RegistryValueKind.DWord);
                key.SetValue("Datei", Datei, RegistryValueKind.String);
                if (!string.IsNullOrEmpty(Parameter))
                    key.SetValue("Parameter", Parameter, RegistryValueKind.String);
                Debug.WriteLine($"create_ZUSI_menu_entry added key data for Fahrplanerstellung {keyVal}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in create_ZUSI_menu_entry_2 {e}");
            }
            finally
            {
                key?.Close();
            }
        }

        
        static public void CreateZUSIMenuEntry(string BezeichnerSprache = "Deutsch", string Bezeichnertext = "", string Vatermenu = "", int MenuIndex = 5, string? Params = "")
        {

            // Check for ZUSI version
            bool noZusiEntry = false;
            bool zusiSteam = false;
            bool checkZusiSteam = false;
            string keyVal = @"Software\Zusi3\Fahrsim\Einstellungen";

            string? executablePath = Process.GetCurrentProcess().MainModule?.FileName;

            if (executablePath != null)
            {
                try
                {
                    using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(keyVal, true))
                    {
                        Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} found");
                        checkZusiSteam = false;
                        zusiSteam = false;
                    }
                }
                catch
                {
                    checkZusiSteam = true;
                }

                if (checkZusiSteam)
                {
                    try
                    {
                        keyVal = @"Software\Zusi3\Fahrsimsteam\Einstellungen";
                        using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(keyVal, true))
                        {
                            Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} found");
                            zusiSteam = true;
                        }
                    }
                    catch
                    {
                        zusiSteam = false;
                        noZusiEntry = true;
                    }
                }

                if (noZusiEntry)
                {
                    Debug.WriteLine("create_ZUSI_menu_entry no ZUSI entry found");
                    return;
                }

                bool zusiKeyOk = true;
                string keyVal0;

                if (zusiSteam)
                {
                    keyVal0 = @"Software\Zusi3\Fahrsimsteam\Einstellungen\Menu"+ Bezeichnertext;
                }
                else
                {
                    keyVal0 = @"Software\Zusi3\Fahrsim\Einstellungen\Menu"+ Bezeichnertext;
                }

                if (zusiKeyOk)
                {
                    ZUSI_write_ext_menuval_to_Regkey(keyVal0, 0, "Deutsch", Bezeichnertext, Vatermenu, MenuIndex, executablePath, Params);
                }
            }
        }

        static public void ZUSI_write_AutoStart_val_to_Regkey(string keyVal, string? DateiName = "", string? Parameter = "")
        {
            RegistryKey? key = null;

            string autostartkey = "";

            bool found = false;

            for (int i = 0; i < 10; i++)

            {
                autostartkey = keyVal + "Autostart" + i.ToString();
                try
                {
                    key = Registry.CurrentUser.OpenSubKey(autostartkey, true);
                    if (key != null)
                    {
                        Debug.WriteLine($"create_ZUSI_AutoStart_entry key {keyVal} found");
                        string? keydata = (string?) key.GetValue("Datei");
                        Debug.WriteLine($"create_ZUSI_AutoStart_entry keydata {keydata}");
                        if (keydata == DateiName)
                        {
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"create_ZUSI_AutoStart_entry key {keyVal} NOT found");
                        found = true; //found a free entry
                        try // create a new entry
                        {
                            key = Registry.CurrentUser.CreateSubKey(autostartkey);
                            Debug.WriteLine($"create_ZUSI_menu_entry key {autostartkey} created");
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"Error in create_ZUSI_menu_entry {e}");
                            return;
                        }
                        break;
                    }
                }
                catch
                { }
            }

            if (key != null && DateiName != null)
            {
                try
                {
                    key.SetValue("Datei", DateiName, RegistryValueKind.String);
 
                    key.SetValue("Passiv", 0, RegistryValueKind.DWord);

                    if (!string.IsNullOrEmpty(Parameter))
                        key.SetValue("Parameter", Parameter, RegistryValueKind.String);
                    Debug.WriteLine($"create_ZUSI_menu_entry added key data {keyVal}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Error in create_ZUSI_AutoStart_entry_2 {e}");
                }
                finally
                {
                    key?.Close();
                }
            }
        }

        static public void CreateZUSIAutoStartEntry(string? DateiName = "", string? Parameter = "")
        {

            // Check for ZUSI version
            bool noZusiEntry = false;
            bool zusiSteam = false;
            bool checkZusiSteam = false;
            string keyVal = @"Software\Zusi3\Fahrsim\Einstellungen";

            //string? executablePath = Process.GetCurrentProcess().MainModule?.FileName;

            
            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(keyVal, true))
                {
                    Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} found");
                    checkZusiSteam = false;
                    zusiSteam = false;
                }
            }
            catch
            {
                checkZusiSteam = true;
            }

            if (checkZusiSteam)
            {
                try
                {
                    keyVal = @"Software\Zusi3\Fahrsimsteam\Einstellungen";
                    using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(keyVal, true))
                    {
                        Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} found");
                        zusiSteam = true;
                    }
                }
                catch
                {
                    zusiSteam = false;
                    noZusiEntry = true;
                }
            }

            if (noZusiEntry)
            {
                Debug.WriteLine("create_ZUSI_menu_entry no ZUSI entry found");
                return;
            }

            bool zusiKeyOk = true;
            string keyVal0;

            if (zusiSteam)
            {
                keyVal0 = @"Software\Zusi3\Fahrsimsteam\Einstellungen";
            }
            else
            {
                keyVal0 = @"Software\Zusi3\Fahrsim\Einstellungen\";
            }

            if (zusiKeyOk)
            {
                ZUSI_write_AutoStart_val_to_Regkey(keyVal0, DateiName, Parameter);
            }
            
        }


        public static string GetZUSIUserfiledir(string defaultFiledir)
        {
            string userDir = "";
            try
            {
                string keyStr = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Zusi3";
                string valueName = "DatenverzeichnisSteam";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyStr, false))
                {
                    if (key != null)
                    {
                        userDir = key.GetValue(valueName)?.ToString();
                    }
                }
            }
            catch
            {
                // Handle exception
            }

            if (string.IsNullOrEmpty(userDir))
            {
                try
                {
                    string keyStr = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Zusi3";
                    string valueName = "DatenverzeichnisSteam";
                    using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(keyStr, false))
                    {
                        if (key != null)
                        {
                            userDir = key.GetValue(valueName)?.ToString();
                        }
                    }
                }
                catch
                {
                    // Handle exception
                }
            }

            if (string.IsNullOrEmpty(userDir))
            {
                try
                {
                    string keyStr = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Zusi3";
                    string valueName = "Datenverzeichnis";
                    using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(keyStr, false))
                    {
                        if (key != null)
                        {
                            userDir = key.GetValue(valueName)?.ToString();
                        }
                    }
                }
                catch
                {
                    // Handle exception
                }

                if (string.IsNullOrEmpty(userDir))
                {
                    try
                    {
                        string keyStr = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Zusi3";
                        string valueName = "Datenverzeichnis";
                        using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(keyStr, false))
                        {
                            if (key != null)
                            {
                                userDir = key.GetValue(valueName)?.ToString();
                            }
                        }
                    }
                    catch
                    {
                        // Handle exception
                    }
                }
            }

            if (string.IsNullOrEmpty(userDir))
            {
                try
                {
                    userDir = GetWindowsUserDir();
                    userDir = Path.Combine(userDir, "Zusi3", "_ZusiData");
                }
                catch
                {
                    userDir = "";
                }
            }

            if (string.IsNullOrEmpty(userDir))
            {
                userDir = defaultFiledir;
            }
            else
            {
                userDir = Path.Combine(userDir, "_Tools", "ZusiMeter");
                Directory.CreateDirectory(userDir);
            }

            return userDir;
        }

        private static string GetWindowsUserDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }



}
