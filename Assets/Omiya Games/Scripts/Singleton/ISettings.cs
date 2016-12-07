using System;

namespace OmiyaGames
{
    public interface ISettings
    {
        bool GetBool(string key, bool defaultValue);
        void SetBool(string key, bool value);

        int GetInt(string key, int defaultValue);
        void SetInt(string key, int value);

        float GetFloat(string key, float defaultValue);
        void SetFloat(string key, float value);

        string GetString(string key, string defaultValue);
        void SetString(string key, string value);

        ENUM GetEnum<ENUM>(string key, ENUM defaultValue) where ENUM : struct, IConvertible;
        void SetEnum<ENUM>(string key, ENUM value) where ENUM : struct, IConvertible;

        DateTime GetDateTime(string key, DateTime defaultValue);
        void SetDateTime(string key, DateTime value);

        TimeSpan GetTimeSpan(string key, TimeSpan defaultValue);
        void SetTimeSpan(string key, TimeSpan value);

        void DeleteAll();
    }
}
