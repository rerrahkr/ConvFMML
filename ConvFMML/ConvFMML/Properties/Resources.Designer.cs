﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConvFMML.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ConvFMML.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   すべてについて、現在のスレッドの CurrentUICulture プロパティをオーバーライドします
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Disabled に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string Disabled {
            get {
                return ResourceManager.GetString("Disabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Enabled に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string Enabled {
            get {
                return ResourceManager.GetString("Enabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Error に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string Error {
            get {
                return ResourceManager.GetString("Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to clock count conversion. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorConverterFailedToIntermediate {
            get {
                return ResourceManager.GetString("ErrorConverterFailedToIntermediate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to convert to MML. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorConverterFailedToMML {
            get {
                return ResourceManager.GetString("ErrorConverterFailedToMML", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Could not read &quot;{0}&quot; because the MIDI was corrupted. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMIDIBroken {
            get {
                return ResourceManager.GetString("ErrorMIDIBroken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to convert MIDI to format 1. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMIDIFormat1 {
            get {
                return ResourceManager.GetString("ErrorMIDIFormat1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to read &quot;{0}&quot;. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMIDIReadFailed {
            get {
                return ResourceManager.GetString("ErrorMIDIReadFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to export to &quot;{0}&quot;. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorMMLFailed {
            get {
                return ResourceManager.GetString("ErrorMMLFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to cut notes and rests by measure. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorModifierCutByBar {
            get {
                return ResourceManager.GetString("ErrorModifierCutByBar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to cut notes and rests by commands. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorModifierCutByCommands {
            get {
                return ResourceManager.GetString("ErrorModifierCutByCommands", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to optimize same place and type commands. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorModifierOptimizeSamePlaceType {
            get {
                return ResourceManager.GetString("ErrorModifierOptimizeSamePlaceType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to optimize same type and value commands. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorModifierOptimizeSameTypeValue {
            get {
                return ResourceManager.GetString("ErrorModifierOptimizeSameTypeValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed part reconstruction. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorModifierPartReconstruct {
            get {
                return ResourceManager.GetString("ErrorModifierPartReconstruct", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to remove commands that does not affect notes. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ErrorModifierRemoveUselessCommands {
            get {
                return ResourceManager.GetString("ErrorModifierRemoveUselessCommands", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Export as に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ExportAs {
            get {
                return ResourceManager.GetString("ExportAs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Conversion failed に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string FailedTitle {
            get {
                return ResourceManager.GetString("FailedTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Select MIDI file to convert に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string InputMIDIDIalogTitle {
            get {
                return ResourceManager.GetString("InputMIDIDIalogTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Control commands に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuControlCommands {
            get {
                return ResourceManager.GetString("MainMenuControlCommands", resourceCulture);
            }
        }
        
        /// <summary>
        ///   General に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuGeneral {
            get {
                return ResourceManager.GetString("MainMenuGeneral", resourceCulture);
            }
        }
        
        /// <summary>
        ///   MML expression (1) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuMMLExpression1 {
            get {
                return ResourceManager.GetString("MainMenuMMLExpression1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   MML expression (2) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuMMLExpression2 {
            get {
                return ResourceManager.GetString("MainMenuMMLExpression2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Note/Rest (1) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuNoteRest1 {
            get {
                return ResourceManager.GetString("MainMenuNoteRest1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Note/Rest(2) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuNoteRest2 {
            get {
                return ResourceManager.GetString("MainMenuNoteRest2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Pan に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuPan {
            get {
                return ResourceManager.GetString("MainMenuPan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Part に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuPart {
            get {
                return ResourceManager.GetString("MainMenuPart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Program change に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuProgramChange {
            get {
                return ResourceManager.GetString("MainMenuProgramChange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Tempo に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuTempo {
            get {
                return ResourceManager.GetString("MainMenuTempo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Volume に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MainMenuVolume {
            get {
                return ResourceManager.GetString("MainMenuVolume", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Keep (c4) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string OverMeasureC4 {
            get {
                return ResourceManager.GetString("OverMeasureC4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Divide (c8&amp;8) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string OverMeasureC8And8 {
            get {
                return ResourceManager.GetString("OverMeasureC8And8", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Divide (c8&amp;c8) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string OverMeasureC8AndC8 {
            get {
                return ResourceManager.GetString("OverMeasureC8AndC8", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Divide (c8^8) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string OverMeasureC8Hat8 {
            get {
                return ResourceManager.GetString("OverMeasureC8Hat8", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Left, Center, Right に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string PanLCR {
            get {
                return ResourceManager.GetString("PanLCR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Use MIDI value に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string PanMIDI {
            get {
                return ResourceManager.GetString("PanMIDI", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Pn (n: 0-255) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string PanP {
            get {
                return ResourceManager.GetString("PanP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   PLn (n: 1-127), PC, PRm (m: 1-127) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string PanPLPCPR {
            get {
                return ResourceManager.GetString("PanPLPCPR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Auto-assign に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string PartAuto {
            get {
                return ResourceManager.GetString("PartAuto", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Custom-assign に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string PartCustom {
            get {
                return ResourceManager.GetString("PartCustom", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Disabled に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string PartDisabled {
            get {
                return ResourceManager.GetString("PartDisabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Exist に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string PartNoteExist {
            get {
                return ResourceManager.GetString("PartNoteExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   None に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string PartNoteNone {
            get {
                return ResourceManager.GetString("PartNoteNone", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Arranging data... に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string StatusBarArrange {
            get {
                return ResourceManager.GetString("StatusBarArrange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Finish conversion に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string StatusBarComplete {
            get {
                return ResourceManager.GetString("StatusBarComplete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Exporting MML... に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string StatusBarExport {
            get {
                return ResourceManager.GetString("StatusBarExport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Conversion failed に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string StatusBarFailed {
            get {
                return ResourceManager.GetString("StatusBarFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Loading &quot;{0}&quot;... に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string StatusBarLoad {
            get {
                return ResourceManager.GetString("StatusBarLoad", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ready に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string StatusBarReady {
            get {
                return ResourceManager.GetString("StatusBarReady", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Changing MML clock count... に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string StatusBarTimeBase {
            get {
                return ResourceManager.GetString("StatusBarTimeBase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Conversion is complete. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string SuccessText {
            get {
                return ResourceManager.GetString("SuccessText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Finish conversion に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string SuccessTitle {
            get {
                return ResourceManager.GetString("SuccessTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   c4&amp;16 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TieLengthStyleC4And16 {
            get {
                return ResourceManager.GetString("TieLengthStyleC4And16", resourceCulture);
            }
        }
        
        /// <summary>
        ///   c4&amp;c16 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TieLengthStyleC4AndC16 {
            get {
                return ResourceManager.GetString("TieLengthStyleC4AndC16", resourceCulture);
            }
        }
        
        /// <summary>
        ///   c4^16 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TieLengthStyleC4Hat16 {
            get {
                return ResourceManager.GetString("TieLengthStyleC4Hat16", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Output as &quot;C&quot; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TimeBasePMDC {
            get {
                return ResourceManager.GetString("TimeBasePMDC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Output as &quot;#Zenlen&quot; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TimeBasePMDZenlen {
            get {
                return ResourceManager.GetString("TimeBasePMDZenlen", resourceCulture);
            }
        }
    }
}
