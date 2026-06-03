using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace AgrXmlWpf
{
  public class MainViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public List<string> WorkTypes { get; } = new()
        {
            "новое строительство",
            "реконструкция",
            "новое строительство и реконструкция"
        };


    public string ObjectName { get; set; }
    public string RegistrationNumber { get; set; }
    public DateTime? Date { get; set; }
    public string Address { get; set; }
    public string UUID { get; set; }
    public string ProjectOrganization { get; set; }
    public string ProjectTeamLead { get; set; }
    public string SelectedWorkType { get; set; }

    public string CadastralNumbersText { get; set; }
    public string GPZUNumbersText { get; set; }
    public string PPTNumbersText { get; set; }
    public string GZKDecisionNumbersText { get; set; }
    public string KRTNumbersText { get; set; }
    public string PPMNumbersText { get; set; }
    public string ProjectTeamMembersText { get; set; }
    public string FunctionalPurposesText { get; set; }

    public string BuildingHeight { get; set; }
    public string Area { get; set; }
    public string TotalFloorArea { get; set; }
    public string ResidentialPartArea { get; set; }
    public string ResidentialPartOfResidentialArea { get; set; }
    public string NonResidentialPartOfResidentialArea { get; set; }
    public string NonResidentialObjectsArea { get; set; }
    public string TotalObjectArea { get; set; }
    public string AboveGroundArea { get; set; }
    public string UndergroundArea { get; set; }
    public string NonResidentialAboveGroundAreaNonResidentialPart { get; set; }
    public string NonResidentialAboveGroundAreaNonResidentialObjects { get; set; }
    public string ApartmentsAreaExSummerPremises { get; set; }
    public string ApartmentsCount { get; set; }
    public string GreenArea { get; set; }
    public string ChildrenPlaygroundArea { get; set; }
    public string RecreationsArea { get; set; }
    public string ObjectHeight { get; set; }
    public string FloorsCount { get; set; }
    public string ApartmentsArea { get; set; }
    public string TotalApartmentsArea { get; set; }
    public string InformationStructuresCount { get; set; }
    public string InformationStructuresParameters { get; set; }

    public ICommand SaveXmlCommand { get; }
    public ICommand FillSampleCommand { get; }

    public MainViewModel()
    {
      SaveXmlCommand = new RelayCommand(_ => SaveXml());
      FillSampleCommand = new RelayCommand(_ => FillSample());
      ClearCommand = new RelayCommand(_ => Clear());
    }

    private void SaveXml()
    {
      try
      {
        ValidateRequiredFields();

        var dialog = new SaveFileDialog
        {
          Filter = "XML files (*.xml)|*.xml",
          FileName = "agr_export.xml"
        };

        if (dialog.ShowDialog() != true)
          return;

        using var writer = XmlWriter.Create(dialog.FileName, new XmlWriterSettings
        {
          Indent = true,
          OmitXmlDeclaration = false
        });

        writer.WriteStartDocument();
        writer.WriteStartElement("ArchitecturalUrbanPlanningSolution");
        writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
        writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "agr_xml_20251211.xsd");

        WriteString(writer, "ObjectName", ObjectName, true);
        WriteString(writer, "RegistrationNumber", RegistrationNumber, false);
        if (Date.HasValue)
          writer.WriteElementString("Date", Date.Value.ToString("yyyy-MM-dd"));
        WriteString(writer, "Address", Address, true);

        WriteList(writer, "CadastralNumbers", "CadastralNumber", CadastralNumbersText);
        WriteList(writer, "GPZUNumbers", "GPZUNumber", GPZUNumbersText);
        WriteList(writer, "PPTNumbers", "PPTNumber", PPTNumbersText);
        WriteList(writer, "GZKDecisionNumbers", "GZKDecisionNumber", GZKDecisionNumbersText);
        WriteList(writer, "KRTNumbers", "KRTNumber", KRTNumbersText);
        WriteList(writer, "PPMNumbers", "PPMNumber", PPMNumbersText);

        WriteString(writer, "UUID", UUID, false);
        WriteString(writer, "ProjectOrganization", ProjectOrganization, true);
        WriteString(writer, "ProjectTeamLead", ProjectTeamLead, true);

        writer.WriteStartElement("ProjectTeam");
        foreach (var member in SplitLines(ProjectTeamMembersText))
          writer.WriteElementString("Member", member);
        writer.WriteEndElement();

        WriteString(writer, "WorkTypes", SelectedWorkType, true);

        writer.WriteStartElement("FunctionalPurposes");
        foreach (var item in SplitLines(FunctionalPurposesText))
          writer.WriteElementString("FunctionalPurpose", item);
        writer.WriteEndElement();

        WriteDecimal(writer, "BuildingHeight", BuildingHeight, true);
        WriteDecimal(writer, "Area", Area, true);
        WriteDecimal(writer, "TotalFloorArea", TotalFloorArea, true);
        WriteDecimal(writer, "ResidentialPartArea", ResidentialPartArea, false);
        WriteDecimal(writer, "ResidentialPartOfResidentialArea", ResidentialPartOfResidentialArea, false);
        WriteDecimal(writer, "NonResidentialPartOfResidentialArea", NonResidentialPartOfResidentialArea, false);
        WriteDecimal(writer, "NonResidentialObjectsArea", NonResidentialObjectsArea, false);
        WriteDecimal(writer, "TotalObjectArea", TotalObjectArea, false);
        WriteDecimal(writer, "AboveGroundArea", AboveGroundArea, false);
        WriteDecimal(writer, "UndergroundArea", UndergroundArea, false);
        WriteDecimal(writer, "NonResidentialAboveGroundAreaNonResidentialPart", NonResidentialAboveGroundAreaNonResidentialPart, false);
        WriteDecimal(writer, "NonResidentialAboveGroundAreaNonResidentialObjects", NonResidentialAboveGroundAreaNonResidentialObjects, false);
        WriteDecimal(writer, "ApartmentsAreaExSummerPremises", ApartmentsAreaExSummerPremises, false);
        WritePositiveInt(writer, "ApartmentsCount", ApartmentsCount, true);
        WriteDecimal(writer, "GreenArea", GreenArea, false);
        WriteDecimal(writer, "ChildrenPlaygroundArea", ChildrenPlaygroundArea, false);
        WriteDecimal(writer, "RecreationsArea", RecreationsArea, false);
        WriteDecimal(writer, "ObjectHeight", ObjectHeight, true);
        WritePositiveInt(writer, "FloorsCount", FloorsCount, true);
        WriteDecimal(writer, "ApartmentsArea", ApartmentsArea, false);
        WriteDecimal(writer, "TotalApartmentsArea", TotalApartmentsArea, false);
        WritePositiveInt(writer, "InformationStructuresCount", InformationStructuresCount, false);
        WriteString(writer, "InformationStructuresParameters", InformationStructuresParameters, false);

        writer.WriteEndElement();
        writer.WriteEndDocument();

        MessageBox.Show("XML успешно сохранён.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void ValidateRequiredFields()
    {
      if (string.IsNullOrWhiteSpace(ObjectName)) throw new Exception("Заполните 'Наименование объекта'.");
      if (string.IsNullOrWhiteSpace(Address)) throw new Exception("Заполните 'Адрес'.");
      if (string.IsNullOrWhiteSpace(ProjectOrganization)) throw new Exception("Заполните 'Проектная организация'.");
      if (string.IsNullOrWhiteSpace(ProjectTeamLead)) throw new Exception("Заполните 'Руководитель авторского коллектива'.");
      if (string.IsNullOrWhiteSpace(SelectedWorkType)) throw new Exception("Выберите 'Виды работ'.");
      if (!SplitLines(ProjectTeamMembersText).Any()) throw new Exception("Добавьте хотя бы одного участника авторского коллектива.");
      if (!SplitLines(FunctionalPurposesText).Any()) throw new Exception("Добавьте хотя бы одно функциональное назначение.");

      ParseDecimal(BuildingHeight, "BuildingHeight");
      ParseDecimal(Area, "Area");
      ParseDecimal(TotalFloorArea, "TotalFloorArea");
      ParsePositiveInt(ApartmentsCount, "ApartmentsCount");
      ParseDecimal(ObjectHeight, "ObjectHeight");
      ParsePositiveInt(FloorsCount, "FloorsCount");
    }

    private static void WriteString(XmlWriter writer, string name, string value, bool required)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        if (required)
          throw new Exception($"Поле '{name}' обязательно.");
        return;
      }

      writer.WriteElementString(name, value);
    }

    private static void WriteDecimal(XmlWriter writer, string name, string value, bool required)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        if (required)
          throw new Exception($"Поле '{name}' обязательно.");
        return;
      }

      writer.WriteElementString(name, ParseDecimal(value, name).ToString(CultureInfo.InvariantCulture));
    }

    private static void WritePositiveInt(XmlWriter writer, string name, string value, bool required)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        if (required)
          throw new Exception($"Поле '{name}' обязательно.");
        return;
      }

      writer.WriteElementString(name, ParsePositiveInt(value, name).ToString(CultureInfo.InvariantCulture));
    }

    private static decimal ParseDecimal(string value, string fieldName)
    {
      var normalized = value.Replace(',', '.');
      if (!decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        throw new Exception($"Поле '{fieldName}' должно быть числом.");
      return result;
    }

    private static int ParsePositiveInt(string value, string fieldName)
    {
      if (!int.TryParse(value, out var result) || result <= 0)
        throw new Exception($"Поле '{fieldName}' должно быть положительным целым числом.");
      return result;
    }

    private static void WriteList(XmlWriter writer, string containerName, string itemName, string rawText)
    {
      var items = SplitLines(rawText).ToList();
      if (!items.Any())
        return;

      writer.WriteStartElement(containerName);
      foreach (var item in items)
        writer.WriteElementString(itemName, item);
      writer.WriteEndElement();
    }

    private static IEnumerable<string> SplitLines(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
        return Enumerable.Empty<string>();

      return text
          .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
          .Select(x => x.Trim())
          .Where(x => !string.IsNullOrWhiteSpace(x));
    }

    private void FillSample()
    {
      ObjectName = "Жилой комплекс \"Примерный\" с торговым центром";
      RegistrationNumber = "РН-2024-001";
      Date = new DateTime(2024, 1, 15);
      Address = "г. Москва, ул. Примерная, д. 1";
      UUID = "UID-77-2024-001";
      ProjectOrganization = "ООО \"Проектная организация \"Архитектурные решения\"\"";
      ProjectTeamLead = "Иванов Иван Иванович";
      SelectedWorkType = "новое строительство";

      CadastralNumbersText = "77:01:0001001:1234\n77:01:0001001:1235";
      GPZUNumbersText = "ГПЗУ-77-001-2023\nГПЗУ-77-001-2024";
      PPTNumbersText = "ППТ-77-001\nППТ-77-002";
      GZKDecisionNumbersText = "ГЗК-2023-0456\nГЗК-2024-0012";
      KRTNumbersText = "КРТ-2024-001";
      PPMNumbersText = "ППМ-77-001\nППМ-77-002\nППМ-77-003";
      ProjectTeamMembersText = "Петров Петр Петрович\nСидоров Сидор Сидорович\nКозлова Клавдия Константиновна";
      FunctionalPurposesText = "многоквартирный жилой дом\nторговый центр\nподземная автостоянка";

      BuildingHeight = "15.5";
      Area = "1.25";
      TotalFloorArea = "25000.75";
      ResidentialPartArea = "18000.50";
      ResidentialPartOfResidentialArea = "16500.25";
      NonResidentialPartOfResidentialArea = "1500.25";
      NonResidentialObjectsArea = "7000.25";
      TotalObjectArea = "30000.75";
      AboveGroundArea = "22000.00";
      UndergroundArea = "8000.75";
      NonResidentialAboveGroundAreaNonResidentialPart = "2000.50";
      NonResidentialAboveGroundAreaNonResidentialObjects = "5000.00";
      ApartmentsAreaExSummerPremises = "16000.00";
      ApartmentsCount = "200";
      GreenArea = "3500.00";
      ChildrenPlaygroundArea = "500.00";
      RecreationsArea = "300.00";
      ObjectHeight = "75.5";
      FloorsCount = "20";
      ApartmentsArea = "16000.00";
      TotalApartmentsArea = "16000.00";
      InformationStructuresCount = "8";
      InformationStructuresParameters = "Наружные вывески с подсветкой, информационные стелы, указатели";

      OnPropertyChanged(string.Empty);
    }
    private void Clear()
    {
      ObjectName = null;
      RegistrationNumber = null;
      Date = null;
      Address = null;
      UUID = null;
      ProjectOrganization = null;
      ProjectTeamLead = null;
      SelectedWorkType = null;

      CadastralNumbersText = null;
      GPZUNumbersText = null;
      PPTNumbersText = null;
      GZKDecisionNumbersText = null;
      KRTNumbersText = null;
      PPMNumbersText = null;
      ProjectTeamMembersText = null;
      FunctionalPurposesText = null;

      BuildingHeight = null;
      Area = null;
      TotalFloorArea = null;
      ResidentialPartArea = null;
      ResidentialPartOfResidentialArea = null;
      NonResidentialPartOfResidentialArea = null;
      NonResidentialObjectsArea = null;
      TotalObjectArea = null;
      AboveGroundArea = null;
      UndergroundArea = null;
      NonResidentialAboveGroundAreaNonResidentialPart = null;
      NonResidentialAboveGroundAreaNonResidentialObjects = null;
      ApartmentsAreaExSummerPremises = null;
      ApartmentsCount = null;
      GreenArea = null;
      ChildrenPlaygroundArea = null;
      RecreationsArea = null;
      ObjectHeight = null;
      FloorsCount = null;
      ApartmentsArea = null;
      TotalApartmentsArea = null;
      InformationStructuresCount = null;
      InformationStructuresParameters = null;

      // обновить UI
      OnPropertyChanged(string.Empty);
    }
    public ICommand ClearCommand { get; }
  }
}
