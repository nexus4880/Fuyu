using System;
using System.Collections.Generic;
using Fuyu.Common.Launcher.Models.Messages;
using Fuyu.Common.Launcher.Models.Settings;

namespace Fuyu.Common.Launcher.Services;

public class SettingsService
{
    public static SettingsService Instance => _instance.Value;
    private static readonly Lazy<SettingsService> _instance = new(() => new SettingsService());

    private readonly List<SettingSection> _settings;

    /// <summary>
    /// The construction of this class is handled in the <see cref="_instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private SettingsService()
    {
        _settings = [];
    }

    public void SetOrAddSection(SettingSection section)
    {
        for (var i = 0; i < _settings.Count; i++)
        {
            if (_settings[i].Id == section.Id)
            {
                _settings[i] = section;
                return;
            }
        }

        _settings.Add(section);
    }

    public string GeneratePage(string htmlTemplate)
    {
        var items = new List<GeneratedSettingItem>();

        foreach (var section in _settings)
        {
            var sectionItems = GenerateSection(section);
            items.AddRange(sectionItems);
        }

        var tocHtml = string.Empty;
        var contentHtml = string.Empty;
        var messageJson = string.Empty;

        foreach (var item in items)
        {
            tocHtml += item.TableOfContents;
            contentHtml += item.Content;
            messageJson += item.Message;
        }

        var page = htmlTemplate
            .Replace("<!-- __TOC __ -->", tocHtml)
            .Replace("<!-- __CONTENT __ -->", contentHtml)
            .Replace("// __MESSAGE__", messageJson);

        return page;
    }

    GeneratedSettingItem[] GenerateSection(SettingSection section)
    {
        var items = new List<GeneratedSettingItem>();

        // add subsection start
        var beginSectionitem = GenerateSectionStart(section);
        items.Add(beginSectionitem);

        // add subsection items 
        foreach (var setting in section.Settings)
        {
            switch (setting.Type)
            {
                case ESettingType.Text:
                    var item = GenerateTextChunk(section, (TextSetting)setting);
                    items.Add(item);
                    break;
            }
        }

        // add subsection end
        var endSectionitem = GenerateSectionEnd(section);
        items.Add(endSectionitem);

        return [.. items];
    }

    GeneratedSettingItem GenerateSectionStart(SettingSection section)
    {
        var item = new GeneratedSettingItem()
        {
            TableOfContents = string.Empty
                + $"<h6 class=\"mt-3\"><a class=\"text-decoration-none\" href=\"#{section.Id}\">{section.Name}</a></h6>\n"
                + $"<ul class=\"list-unstyled ps-3\">\n",

            Content = string.Empty
                + $"<h3 id=\"{section.Id}\">{section.Name}</h3>\n"
        };

        return item;
    }

    GeneratedSettingItem GenerateSectionEnd(SettingSection section)
    {
        var item = new GeneratedSettingItem()
        {
            TableOfContents = string.Empty
                + $"</ul>\n",
        };

        return item;
    }

    GeneratedSettingItem GenerateTextChunk(SettingSection section, TextSetting setting)
    {
        var id = $"{section.Id}-{setting.Id}";
        var item = new GeneratedSettingItem()
        {
            TableOfContents = string.Empty
                + $"<li><a class=\"text-decoration-none\" href=\"#{id}\">{setting.Name}</a></li>\n",

            Content = string.Empty
                + "<div class=\"mb-3\">\n"
                + $"    <label for=\"{id}\" class=\"form-label\">{setting.Name}</label>\n"
                + $"    <input class=\"form-control\" type=\"text\" value=\"{setting.Value}\" id=\"{id}\">\n"
                + $"    <div class=\"form-text\">{setting.Description}</div>\n"
                + "</div>\n",

            Message = string.Empty
                + "{\n"
                + $"    id: \"{id}\","
                + $"    value: document.getElementById(\"{id}\").value\n"
                + "},\n"
        };

        return item;
    }

    public void SaveSettings(SaveSettingsMessage message)
    {
        foreach (var entry in message.Data)
        {
            SaveSetting(entry);
        }
    }

    void SaveSetting(SaveSettingsEntry entry)
    {
        var splitted = entry.Id.Split('-');
        var sectionId = splitted[0];
        var settingId = splitted[1];
        Setting target = null;

        foreach (var section in _settings)
        {
            if (section.Id == sectionId)
            {
                foreach (var setting in section.Settings)
                {
                    if (setting.Id == settingId)
                    {
                        target = setting;
                        goto search_end;
                    }
                }
            }
        }
    search_end:

        if (target == null)
        {
            throw new Exception($"Could not find setting {settingId} in section {sectionId}");
        }

        target.OnSave(entry.Value);
    }
}