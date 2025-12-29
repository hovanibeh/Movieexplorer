using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MovieExplorer.Models;

namespace MovieExplorer.Services
{
    // This class handles saving and loading user data
    public class DataService
    {
        // Where to save user profile file
        private readonly string _profileFilePath;

        // Where to save settings file
        private readonly string _settingsFilePath;

        // Constructor - sets up file paths
        public DataService()
        {
            // FileSystem.AppDataDirectory = our app's special folder
            _profileFilePath = Path.Combine(FileSystem.AppDataDirectory, "user_profile.json");
            _settingsFilePath = Path.Combine(FileSystem.AppDataDirectory, "app_settings.json");
        }

        // ========== USER PROFILE METHODS ==========

        // Save user profile to file
        public async Task SaveProfileAsync(UserProfile profile)
        {
            try
            {
                // Convert UserProfile object to JSON text
                var options = new JsonSerializerOptions { WriteIndented = true }; // Makes it pretty
                string json = JsonSerializer.Serialize(profile, options);

                // Write to file
                await File.WriteAllTextAsync(_profileFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving profile: {ex.Message}");
            }
        }

        // Load user profile from file
        public async Task<UserProfile> LoadProfileAsync()
        {
            try
            {
                // Does the file exist?
                if (File.Exists(_profileFilePath))
                {
                    // YES - Read and convert JSON to UserProfile object
                    string json = await File.ReadAllTextAsync(_profileFilePath);
                    var profile = JsonSerializer.Deserialize<UserProfile>(json);
                    return profile ?? new UserProfile(); // Return profile or new empty one
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading profile: {ex.Message}");
            }

            // If file doesn't exist or error, return new empty profile
            return new UserProfile();
        }

        // ========== SETTINGS METHODS ==========

        // Save app settings to file
        public async Task SaveSettingsAsync(AppSettings settings)
        {
            try
            {
                // Convert AppSettings object to JSON text
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(settings, options);

                // Write to file
                await File.WriteAllTextAsync(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        // Load app settings from file
        public async Task<AppSettings> LoadSettingsAsync()
        {
            try
            {
                // Does the file exist?
                if (File.Exists(_settingsFilePath))
                {
                    // YES - Read and convert JSON to AppSettings object
                    string json = await File.ReadAllTextAsync(_settingsFilePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    return settings ?? new AppSettings(); // Return settings or new empty one
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            // If file doesn't exist or error, return new default settings
            return new AppSettings();
        }
    }
}