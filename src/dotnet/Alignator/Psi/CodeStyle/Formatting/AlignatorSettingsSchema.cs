#nullable enable

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Components;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.OptionPages;
using JetBrains.ReSharper.Feature.Services.OptionPages.CodeStyle;
using JetBrains.ReSharper.Feature.Services.OptionPages.CodeStyle.ViewModels;
using JetBrains.ReSharper.Features.Altering.CodeFormatter.CSharp;
using JetBrains.ReSharper.Features.Altering.CodeStyle.CSharp;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

// public class Cake : CSharpIndentStylePageSchemaBase<AlignatorSettingsKey>
// {
//     public Cake(Lifetime lifetime, IContextBoundSettingsStoreLive smartContext, IValueEditorViewModelFactory itemViewModelFactory, IComponentContainer container, ISettingsToHide settingsToHide, ICalculatedSettingsSchema calculatedSettingsSchema)
//         : base(lifetime, smartContext, itemViewModelFactory, container, settingsToHide, calculatedSettingsSchema)
//     {
//     }
// }

[FormattingSettingsPresentationComponent]
public class AlignatorSettingsSchema : OthersPageSchemaPart
{
    public AlignatorSettingsSchema(Lifetime                       lifetime,
                                   IContextBoundSettingsStoreLive smartContext,
                                   IValueEditorViewModelFactory   itemViewModelFactory,
                                   IComponentContainer            container,
                                   ISettingsToHide                settingsToHide,
                                   ICalculatedSettingsSchema      calculatedSettingsSchema
    )
        : base(lifetime, smartContext, itemViewModelFactory, container, settingsToHide, calculatedSettingsSchema)
    {
    }

    protected override string PagePartName => "Alignator align arguments KUCHEN";

    protected override void DescribePart(SchemaBuilder builder)
    {
        // The formatter runs over the code that is given to the setting inside ItemFor.
        // It is used to demo the setting's effect.

        builder
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
                    """
                    static void DefinedType()
                    {
                      var SettingsSortIndex = 1;

                      var coreName = new StringSetting("-1657-4d40-9c17-dcade0c6415d",       "Core Name",                   ++SettingsSortIndex, true, HtmlFieldType.Text, "Core 1");
                      var apiAddress = new StringSetting("a111683e--443d-b8fc-4407a5fb14ba", "Core IP-Address or Hostname", ++SettingsSortIndex, true, HtmlFieldType.Url,  "127.0.0.1") {HtmlFieldWidth = 6};
                      var apiPort = new UShortSetting("f831819e-61c4-4915--9b6adceb46fd", "Listen Port API", ++SettingsSortIndex, true, 8085) {HtmlFieldWidth                                           = 6};
                      var pulseDuration = new IntSetting("bfdde1fe-311f-47d0-9ad3-", "Pulse Duration", ++SettingsSortIndex, true, 250) {Description                                                     = "In ms"};
                      var useWebSocketsOverTls = new BoolSetting("-4c86-a54d-0b64a40fe8d5", "Use WebSockets over TLS", ++SettingsSortIndex, false) {Description                                         = "WebSocket Secure"};
                      var webSocketPort = new UShortSetting("c89bc913-", "WebSocket Port", ++SettingsSortIndex, true, 4444);
                      var shutdownActions = new StringSetting("9acd45ec-e8b9-45ad-b136-ecdeecd87074", "Shutdown Actions", ++SettingsSortIndex, false, HtmlFieldType.Text, null);
                      var startupActions = new StringSetting("caf5ff1a5983",                          "Startup Actions",  ++SettingsSortIndex, false, HtmlFieldType.Text, null);
                    }

                    static void TargetType()
                    {
                      var settingsSortIndex = 1;

                      StringSetting coreName = new("-1657-4d40-9c17-dcade0c6415d", "Core Name", ++settingsSortIndex, true, HtmlFieldType.Text, "Core 1");
                      StringSetting apiAddress = new("a111683e--443d-b8fc-4407a5fb14ba", "Core IP-Address or Hostname", ++settingsSortIndex, true, HtmlFieldType.Url, "127.0.0.1") {HtmlFieldWidth = 6};
                      UShortSetting apiPort = new("f831819e-61c4-4915--9b6adceb46fd", "Listen Port API", ++settingsSortIndex, true, 8085) {HtmlFieldWidth                                          = 6};
                      IntSetting pulseDuration = new("bfdde1fe-311f-47d0-9ad3-", "Pulse Duration", ++settingsSortIndex, true, 250) {Description                                                    = "In ms"};
                      BoolSetting useWebSocketsOverTls = new("-4c86-a54d-0b64a40fe8d5", "Use WebSockets over TLS", ++settingsSortIndex, false) {Description                                        = "WebSocket Secure"};
                      UShortSetting webSocketPort = new("c89bc913-", "WebSocket Port", ++settingsSortIndex, true, 4444);
                      StringSetting shutdownActions = new("9acd45ec-e8b9-45ad-b136-ecdeecd87074", "Shutdown Actions", ++settingsSortIndex, false, HtmlFieldType.Text, null);
                      StringSetting startupActions = new("caf5ff1a5983", "Startup Actions", ++settingsSortIndex, false, HtmlFieldType.Text, null);
                    }
                    """,
                    parseType: PreviewParseType.Member,
                    previewFixup: IntAlignFixup)
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENTS_IN_FUNCTION,
                    """
                            static void CallFunctions()
                            {
                              Caller.F1("T-Rex",  "Raptor",        "Moros",         "Triceratops");
                              Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
                              Caller.F2("Gallimimus", "Indor", "Carno", "Spino");
                              Caller.F1("Veloci", "Baryo", "Moso", "Dilophosaurus");
                              Caller.F3("T-Rex", "Raptor", "Moros", "Triceratops");
                              Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
                              Caller.F4("Gallimimus", "Indor", "Carno", "Spino");
                              Caller.F1("Veloci", "Baryo", "Moso", "Dilophosaurus");
                            }
                    """)
            ;

        // Don't call Build() here, it will complain.
        // Build() is called in the parent schema's Describe() method.
    }

    private void IntAlignFixup(IContextBoundSettingsStore boundSettingsStore)
    {
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, bool>((Expression<Func<CSharpFormatSettingsKey, bool>>)(key => key.KEEP_USER_LINEBREAKS),                         true);
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, bool>((Expression<Func<CSharpFormatSettingsKey, bool>>)(key => key.WRAP_LINES),                                   false);
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, bool>((Expression<Func<CSharpFormatSettingsKey, bool>>)(key => key.PLACE_SIMPLE_METHOD_ON_SINGLE_LINE),           true);
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, bool>((Expression<Func<CSharpFormatSettingsKey, bool>>)(key => key.PLACE_ABSTRACT_ACCESSORHOLDER_ON_SINGLE_LINE), true);
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, PlaceOnSameLineAsOwner>((Expression<Func<CSharpFormatSettingsKey, PlaceOnSameLineAsOwner>>)(key => key.PLACE_ACCESSORHOLDER_ATTRIBUTE_ON_SAME_LINE_EX), PlaceOnSameLineAsOwner.ALWAYS);
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, PlaceOnSameLineAsOwner>((Expression<Func<CSharpFormatSettingsKey, PlaceOnSameLineAsOwner>>)(key => key.PLACE_METHOD_ATTRIBUTE_ON_SAME_LINE_EX),         PlaceOnSameLineAsOwner.IF_OWNER_IS_SINGLE_LINE);
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, PlaceOnSameLineAsOwner>((Expression<Func<CSharpFormatSettingsKey, PlaceOnSameLineAsOwner>>)(key => key.PLACE_FIELD_ATTRIBUTE_ON_SAME_LINE_EX),          PlaceOnSameLineAsOwner.ALWAYS);
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, PlaceOnSameLineAsOwner>((Expression<Func<CSharpFormatSettingsKey, PlaceOnSameLineAsOwner>>)(key => key.PLACE_RECORD_FIELD_ATTRIBUTE_ON_SAME_LINE_EX),   PlaceOnSameLineAsOwner.ALWAYS);
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, NestedTernaryExpressionStyle>((Expression<Func<CSharpFormatSettingsKey, NestedTernaryExpressionStyle>>)(key => key.NESTED_TERNARY_STYLE), NestedTernaryExpressionStyle.AUTODETECT);
    }
}